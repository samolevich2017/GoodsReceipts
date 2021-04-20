using System;

namespace Utils {
    public class Utils {

        // Подготовка консоли к работе
        public static void Init(string title) {
            Console.Title = title;
            SetColor(ConsoleColor.White, ConsoleColor.Black);
            Console.SetWindowSize(120, 35);
            Console.SetBufferSize(120, 1000);
            Console.Clear();
        } // Init

        // Формирование случайных чисел
        private static Random _random = new Random();

        public static int GetRandom(int lo, int hi) {
            return _random.Next(lo, hi + 1);
        } // GetRandom

        public static double GetRandom(double lo, double hi) {
            return lo + (hi - lo) * _random.NextDouble();
        } // GetRandom


        // Вывод строки в конкретную позицию консоли
        public static void WriteXY(int x, int y, string str) {
            Console.SetCursorPosition(x, y);
            Console.Write(str);
        } // WriteXY


        // Ожидание нажатия на любую клавишу
        public static ConsoleKey WaitForKey(string text = "Нажмите любую клавишу для продолжения...") {
            Console.Write(text);
            ConsoleKeyInfo key = Console.ReadKey();
            return key.Key;
        } // WaitForKey


        // Ввод строки текста
        public static string GetText(string prompt, int x, int y,
            ConsoleColor fore, ConsoleColor back) {
            WriteXY(x, y, prompt);

            // Цвет строки ввода и вывод шаблона строки ввода
            SaveColor();
            SetColor(fore, back);
            WriteXY(x, y + 1, new string(' ', 60));

            // Курсор в начало "строки ввода" и ввод
            Console.SetCursorPosition(x, y + 1);
            string str = Console.ReadLine();
            RestoreColor();

            return str;
        } // GetText


        // место для сохранения цвета консоли
        private static ConsoleColor _storeBackColor, _storeForeColor;

        // Сохранить цвет консоли
        public static void SaveColor() {
            _storeBackColor = Console.BackgroundColor;
            _storeForeColor = Console.ForegroundColor;
        } // SaveColor


        // Установить цвет консоли
        public static void SetColor(ConsoleColor foreColor, ConsoleColor backColor) {
            Console.BackgroundColor = backColor;
            Console.ForegroundColor = foreColor;
        } // SetColor


        // Восстановить сохраненный цвет консоли
        public static void RestoreColor() {
            Console.BackgroundColor = _storeBackColor;
            Console.ForegroundColor = _storeForeColor;
        } // RestoreColor

    } // class Utils
} // namespace Utils
