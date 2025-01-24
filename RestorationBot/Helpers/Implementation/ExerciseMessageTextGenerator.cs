namespace RestorationBot.Helpers.Implementation;

using Abstract;
using Contracts;
using Shared.Enums;

public class ExerciseMessageTextGenerator : IExerciseMessageTextGenerator
{
    public string GenerateExerciseMessageText(ExerciseMessageInformation messageInformation)
    {
        return messageInformation.RestorationStep switch
        {
            RestorationSteps.Early => messageInformation.ExerciseIndex switch
            {
                1 => """
                     Дыхательные упражнения:

                     1. Глубокий вдох через нос, затем медленный выдох через рот. 
                     Повторите 10 раз

                     2. Дышите с сопротивлением: выдыхайте через губы, сложенные трубочкой. 
                     Повторите 10 раз

                     3. "Плавное дыхание: вдох на 3 секунды, задержка на 2 секунды, выдох на 4 секунды. 
                     Повторите 8–10 раз
                     """,
                2 => """
                     Изометрические упражнения для квадрицепсов:

                     Лежа на спине, напрягите мышцы передней поверхности бедра. Удерживайте напряжение 5 секунд, затем расслабьтесь. 
                     Повторите 10 раз
                     """,
                3 => """
                     Изометрические упражнения для ягодичных мышц:

                     Лежа на спине, напрягите мышцы ягодиц. Удерживайте напряжение 5 секунд, затем расслабьтесь. 
                     Повторите 10 раз
                     """,
                4 => """
                     Упражнения для стоп и пальцев ног:

                     1. Сгибайте и разгибайте стопы. 
                     Повторите 10 раз

                     2. Выполняйте круговые движения стопами. 
                     Повторите 10 раз в каждую сторону

                     3. Сжимайте и разжимайте пальцы ног. 
                     Повторите 10 раз
                     """,
                _ => throw new ArgumentOutOfRangeException()
            },
            RestorationSteps.Middle => messageInformation.ExerciseIndex switch
            {
                1 => """
                     Активное сгибание и разгибание в коленном суставе:

                     Лежа на спине или сидя, сгибайте и разгибайте колено медленно и плавно. 
                     Повторите 10 раз
                     """,
                2 => """
                     Поднятие прямой ноги:

                     Лежа на спине, поднимите прямую ногу на 15–20 см от пола. Удерживайте её 5 секунд, затем опустите. 
                     Повторите 10 раз
                     """,
                3 => """
                     Упражнения с резиновой лентой:

                     Закрепите резинку вокруг стопы. Сгибайте и разгибайте ногу, создавая небольшое сопротивление. 
                     Повторите 10 раз
                     """,
                4 => """
                     Формирование правильной походки:

                     Начните ходить с тростью или костылями. Ставьте стопу плавно, перекатывая её с пятки на носок. 
                     Постепенно увеличивайте длительность ходьбы
                     """,
                _ => throw new ArgumentOutOfRangeException()
            },
            RestorationSteps.Late => messageInformation.ExerciseIndex switch
            {
                1 => """
                     Приседания с опорой:

                     Держась за спинку стула, выполните неглубокие приседания. Следите, чтобы колени не выходили за линию носков. 
                     Повторите 10 раз
                     """,
                2 => """
                     Подъёмы на носки:

                     Поднимайтесь на носки и опускайтесь в исходное положение. 
                     Повторите 10 раз
                     """,
                3 => """
                     Баланс на одной ноге:

                     Стоя на одной ноге, удерживайте равновесие 10–15 секунд.
                     Повторите по 5 раз на каждую ногу
                     """,
                4 => """
                     Ходьба:

                     Увеличивайте время и расстояние ходьбы. Старайтесь ходить без поддержки, если чувствуете уверенность
                     """,
                _ => throw new ArgumentOutOfRangeException()
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}