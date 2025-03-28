namespace RestorationBot.Telegram.Handlers.Callback.Implementation.UserTraining;

using System.Text.RegularExpressions;
using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserTraining;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using Helpers.Abstract;
using Helpers.Abstract.MessageGenerators;
using Helpers.Models.Request;
using Helpers.Models.Response;
using Shared.Enums;

public class ExerciseTypeChoosingCallbackHandler : ICallbackHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IExerciseMessageGenerator _exerciseMessageGenerator;
    private readonly IIdeomotorExerciseMessageGenerator _ideomotorExerciseMessageGenerator;
    private readonly ILogger<ExerciseTypeChoosingCallbackHandler> _logger;
    private readonly IUserTrainingStateStorageService _storageService;

    public ExerciseTypeChoosingCallbackHandler(ICallbackGenerator callbackGenerator,
                                               IUserTrainingStateStorageService storageService,
                                               IExerciseMessageGenerator exerciseMessageGenerator,
                                               ILogger<ExerciseTypeChoosingCallbackHandler> logger,
                                               IIdeomotorExerciseMessageGenerator ideomotorExerciseMessageGenerator)
    {
        _callbackGenerator = callbackGenerator;
        _storageService = storageService;
        _exerciseMessageGenerator = exerciseMessageGenerator;
        _logger = logger;
        _ideomotorExerciseMessageGenerator = ideomotorExerciseMessageGenerator;
    }

    public bool CanHandle(CallbackQuery callbackQuery)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnGetExercise().Match(callbackQuery.Data!);
        UserTrainingState state = _storageService.GetOrAddState(callbackQuery.From.Id);

        return match.Success &&
               state.StateMachine.State == UserTrainingStateProfile.ExerciseTypeChoosing;
    }

    public async Task HandleCommandAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
                                         CancellationToken cancellationToken)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnGetExercise().Match(callbackQuery.Data!);
        if (!match.Success) throw new ArgumentException("Invalid callback query");

        RestorationSteps restorationStep =
            (RestorationSteps)int.Parse(match.Groups["step"].Value);
        int exerciseStep = int.Parse(match.Groups["index"].Value);

        ExerciseMessageInformation message = ExerciseMessageInformation.Create(restorationStep, exerciseStep);
        await OnGettingExerciseAsync(callbackQuery.From, message, botClient, cancellationToken);
    }

    private async Task OnGettingExerciseAsync(User userFrom, ExerciseMessageInformation result,
                                              ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        UserTrainingState state = _storageService.GetOrAddState(userFrom.Id);

        _logger.LogInformation("User with id {userId}, have ExercisePointChosen: {point chosen}", userFrom.Id,
            state.ExercisePointChosen);

        switch (state.ExercisePointChosen)
        {
            case 0:
                await SendIdeomotorExerciseMessagesAsync(userFrom, result, botClient, cancellationToken);
                break;
            case 1:
                await SendPhysicalExerciseMessagesAsync(userFrom, result, botClient, cancellationToken);
                break;
            default: throw new ArgumentException("Invalid exercise point");
        }

        state.ExerciseTypeChosen = result.ExerciseType;

        const string messageOnGettingHeartRate = """
                                                 Каким был ваш показатель частоты сердечных сокращений после выполнения тренировки?
                                                 """;
        await botClient.SendMessage(userFrom.Id, messageOnGettingHeartRate, ParseMode.Html,
            cancellationToken: cancellationToken);
        await state.StateMachine.FireAsync(UserTrainingTriggerProfile.ExerciseTypeChosen);
    }

    private async Task SendIdeomotorExerciseMessagesAsync(User userFrom,
                                                          ExerciseMessageInformation messageInformation,
                                                          ITelegramBotClient botClient,
                                                          CancellationToken cancellationToken)
    {
        string message = _ideomotorExerciseMessageGenerator.GenerateMessage(messageInformation)
                      ?? throw new ArgumentException("Invalid ideomotor exercise message");

        await botClient.SendMessage(userFrom.Id, message, ParseMode.Html, cancellationToken: cancellationToken);
    }

    private async Task SendPhysicalExerciseMessagesAsync(User userFrom,
                                                         ExerciseMessageInformation messageInformation,
                                                         ITelegramBotClient botClient,
                                                         CancellationToken cancellationToken)
    {
        IEnumerable<TelegramMessageWithVideoFiles>
            messages = _exerciseMessageGenerator.GenerateExerciseMessages(messageInformation);

        foreach (TelegramMessageWithVideoFiles message in messages)
        {
            switch (message.Videos.Count)
            {
                case 0:
                    await botClient.SendMessage(userFrom.Id, message.Text, ParseMode.Html,
                        cancellationToken: cancellationToken);
                    continue;
                case > 10:
                    throw new ArgumentException("Too many animation provided");
                case 1:
                {
                    InputFile animation = message.Videos.First();
                    await botClient.SendVideo(userFrom.Id, animation, message.Text, ParseMode.Html,
                        cancellationToken: cancellationToken);

                    continue;
                }
            }

            List<InputMediaVideo> videos = message.Videos.Select(x => new InputMediaVideo(x))
                                                  .ToList();
            videos[0].Caption = message.Text;

            IEnumerable<IAlbumInputMedia> media = videos;

            await botClient.SendMediaGroup(userFrom.Id, media, cancellationToken: cancellationToken);
        }
    }
}