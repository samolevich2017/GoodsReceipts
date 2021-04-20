using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Runtime.Serialization.Json;

namespace ExamProject {
    class Program {

        static string filePath = @"..\..\Receipts.json"; // файл для сохранения данных
        static List<Receipt> Receipts = new List<Receipt>(); // коллекция чеков
        static void Main(string[] args) {

            // подготовка консоли
            Utils.Utils.Init("Экзаменационная работа по C#.NET (IvanSamolevich)");

            string menu = "\n\n\t --- Меню ---\n" +
                "\n 1 - Вывод чеков" +
                "\n 2 - Добавление товара" +
                "\n 3 - Изменение товара" +
                "\n 4 - Удаление товара" +
                "\n 5 - Поиск информации о товаре по его коду" +
                "\n 6 - Загрузить данные из файла" +
                "\n 7 - Сохранить данные в файл" +
                "\n --------------------------------------------------"+
                "\n 8 - LINQ_запросы " +
                "\n --------------------------------------------------" +
                "\n 9 - ***Запасной вариант заполнения коллекции***" +
                "\n Esc - Завершение работы приложения" +
                "\n   Ваше действие --> ";

            try {
                while (true) {
                    Console.Clear();
                    Console.Write(menu);
                    ConsoleKeyInfo c = Console.ReadKey();

                    switch (c.Key) {

                        case ConsoleKey.D1: // вывод чеков
                        case ConsoleKey.NumPad1:
                            Console.Clear();
                            DisplayAllReceipts(Receipts);
                            break;

                        case ConsoleKey.D2: // добавление товара
                        case ConsoleKey.NumPad2:
                            Console.Clear();
                            AddingGoodsInReceipts(Receipts);
                            break;

                        case ConsoleKey.D3: // изменение товара
                        case ConsoleKey.NumPad3:
                            UpdateGoodsInReceipt(Receipts);
                            Console.Clear();
                            break;

                        case ConsoleKey.D4: // удаление товара
                        case ConsoleKey.NumPad4:
                            Console.Clear();
                            DeleteGoodsFromReceipt(Receipts);
                            break;

                        case ConsoleKey.D5: // поиск по коду
                        case ConsoleKey.NumPad5:
                            Console.Clear();
                            InfoAboutGoodsByCode(Receipts);
                            break;

                        case ConsoleKey.D6: // загрузка данный из файла
                        case ConsoleKey.NumPad6:
                            Console.WriteLine("\n   Загружаю данные...");
                            ReadFromJSON(filePath); // чтение из файла
                            break;

                        case ConsoleKey.D7: // выгрузка данных из файла
                        case ConsoleKey.NumPad7:
                            Console.WriteLine("\n   Сохраняю данные...");
                            WriteToJSON(filePath); // запись в файл
                            break;

                        case ConsoleKey.D8: // отдельное меню с запросами
                        case ConsoleKey.NumPad8:
                            Console.Clear();
                            MenuLINQ(Receipts);
                            break;

                        case ConsoleKey.D9:
                        case ConsoleKey.NumPad9:
                            Console.WriteLine("\n   Заполняю коллекцию данных своими силами...");
                            ProgressBar();
                            InitializeReceipts(); // инициализация списка
                            break;

                        case ConsoleKey.Escape: // завершение работы приложения
                            Environment.Exit(0);
                            break;

                        default:
                            ConsoleColor oldCol = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n !!! Такого пункта меню не существует !!!");
                            Utils.Utils.WaitForKey("Чтобы повторить попытку, нажмите любую клавишу...");
                            Console.ForegroundColor = oldCol;
                            break;

                    } // switch

                } // while

            } catch(Exception e) {
                Console.WriteLine($"{e.Message}");
            } // try/catch

        } // Main

        #region Запись/Чтение JSON

        // загрузка данных в файл JSON
        static void WriteToJSON(string filePath) {

            // проверка - есть ли что записывать?
            if (Receipts.Count <= 0) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Упс... Я не знаю что сохранять =( \nКоллекция данных пуста!");
                Console.ForegroundColor = oldCol;
                Utils.Utils.WaitForKey();
                return;
            } // if

            ProgressBar(); // эмуляция загрузки

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Receipt>));

            // записываем в указанный файл, если его нет, создаём и пишем
            using(FileStream fs = new FileStream(filePath, FileMode.Create)) {
                jsonFormatter.WriteObject(fs, Receipts);
            } // using

        } // WriteToJSON

        // выгрузка данных из файла JSON
        static void ReadFromJSON(string filePath) {

            if (!File.Exists(filePath)) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" К сожалению, мне не удалось найти файл для выгрузки данных =(");
                Console.ForegroundColor = oldCol;
                Utils.Utils.WaitForKey();
                return;
            } // if

            ProgressBar(); // эмуляция загрузки

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Receipt>));

            using(FileStream fs = new FileStream(filePath, FileMode.Open)) {
                Receipts = (List<Receipt>)jsonFormatter.ReadObject(fs);
            } // using

        } // ReadFromJSON

        #endregion

        #region LINQ - запросы

        // Запрос 1 - Товары из чеков с заданным диапазоном цен
        static void GoodsRadiusPrice(List<Receipt> receipts) {

            // минимальная и максимальная цена
            // задаём диапазон цен
            Console.Write("Введите диапазон цен : \n Минимальная = ");
            double minPrice = Convert.ToDouble(Console.ReadLine());
            Console.Write(" Максимальная = ");
            double maxPrice = Convert.ToDouble(Console.ReadLine());

            // список выбираемых товаров
            List<Goods> res = new List<Goods>();

            // выбираем элементы соответствующие условию
            for (int i = 0; i < receipts.Count; i++) {
                // собственно сам запрос
                 var go = receipts[i].ListGoods
                     .Where(g => g.PriceG >= minPrice && g.PriceG <= maxPrice)
                     .ToList();

                res.AddRange(go); // добавление коллекций товаров в список
            } // for

            // вывод товаров
            foreach (var item in res)
                item.DisplayInfo();

            Utils.Utils.WaitForKey();
        } // GoodsRadiusPrice

        // Запрос 2 - Средняя сумма по чеку
        static void AvgSumReceipt(List<Receipt> receipts) {
            try {
                Console.Write($"\nКакой из {receipts.Count} чеков, вас интересует?\n Номер --> ");
                int indexSelectedReceipt = Convert.ToInt32(Console.ReadLine()) - 1;

                // если введенный индекс выходит за границы коллекции - бросаем исключение
                if (indexSelectedReceipt > receipts.Count)
                    throw new IndexOutOfRangeException();

                // вывод выбранного пользователем чека
                Console.Clear();
                Console.WriteLine(" Ниже представлен выбранный Вами чек...");
                receipts[indexSelectedReceipt].DisplayInfo();

                // средняя сумма по чеку
                double avgSum = receipts[indexSelectedReceipt].ListGoods.Average(g => g.PriceG);
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n Средняя сумма по чеку = {avgSum:N2} руб.\n\n");
                Console.ForegroundColor = oldCol;
            }
            catch (IndexOutOfRangeException) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Введенный номер превысил максимально допустимое значение!");
                Console.ForegroundColor = oldCol;
            }
            catch (Exception ex) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{ex.Message}");
                Console.ForegroundColor = oldCol;
            } // try/catch

            Utils.Utils.WaitForKey();
        } // AvgPriceReceipt

        // Запрос 3 - Общая информация по коллекции чеков 
        // – товар, суммарное количество, сумма к оплате
        static void AllInfoForReceipts() {
            Console.Clear();
            ConsoleColor oldCol = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" К сожалению, данный запрос не реализован =( \n Прошу прощения.");
            Console.ForegroundColor = oldCol;
            Utils.Utils.WaitForKey();
        } // AllInfoForReceipts

        // Запрос 4 - Товар/товары с минимальным количеством
        static void GoodsWithMinAmount(List<Receipt> receipts) {

            try {
                Console.Write($"\nКакой из {receipts.Count} чеков, вас интересует?\n Номер --> ");
                int indexSelectedReceipt = Convert.ToInt32(Console.ReadLine()) - 1;

                // если введенный индекс выходит за границы коллекции - бросаем исключение
                if (indexSelectedReceipt > receipts.Count)
                    throw new IndexOutOfRangeException();

                // вывод выбранного пользователем чека
                Console.Clear();
                Console.WriteLine(" Ниже представлен выбранный Вами чек...");
                receipts[indexSelectedReceipt].DisplayInfo();

                // список выбираемых товаров
                List<Goods> res = new List<Goods>();

                // минимальное количество
                int min = receipts[indexSelectedReceipt].ListGoods.Min(g => g.AmountG);

                Console.WriteLine("\n\n Товары с минимальным количеством : ");
                // выбираем элементы соответствующие условию
                for (int i = 0; i < receipts.Count; i++) {
                    // собственно сам запрос
                    var go = receipts[i].ListGoods
                        .Where(g => g.AmountG == min)
                        .ToList();

                    res.AddRange(go); // добавление коллекций товаров в список
                } // for

                // вывод товаров
                foreach (var item in res)
                    item.DisplayInfo();

            }
            catch (IndexOutOfRangeException) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Введенный номер превысил максимально допустимое значение!");
                Console.ForegroundColor = oldCol;
            }
            catch (Exception ex) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{ex.Message}");
                Console.ForegroundColor = oldCol;
            } // try/catch

            Utils.Utils.WaitForKey();
        } // GoodsWithMinAmount

        // меню для запросов
        static void MenuLINQ(List<Receipt> receipts) {
            string menu = "\n\t --- Меню запросов ---\n" +
                " Запрос 1 - Товары из чеков с заданным диапазоном цен\n" +
                " Запрос 2 - Средняя сумма по чеку\n" +
                " Запрос 3 - Общая информация по коллекции чеков\n" +
                " Запрос 4 - Товар/товары с минимальным количеством\n" +
                " Esc - выйти в главное меню\n"+
                " Ваш выбор --> ";

            while (true) {
                Console.Clear();
                Console.Write(menu);
                ConsoleKeyInfo c = Console.ReadKey();

                switch (c.Key) {

                    case ConsoleKey.D1: // Запрос 1
                    case ConsoleKey.NumPad1:
                        Console.Clear();
                        GoodsRadiusPrice(receipts);
                        break;

                    case ConsoleKey.D2: // Запрос 2
                    case ConsoleKey.NumPad2:
                        Console.Clear();
                        AvgSumReceipt(receipts);
                        break;

                    case ConsoleKey.D3: // Запрос 3
                    case ConsoleKey.NumPad3:
                        AllInfoForReceipts();
                        Console.Clear();
                        break;

                    case ConsoleKey.D4: // Запрос 4
                    case ConsoleKey.NumPad4:
                        Console.Clear();
                        GoodsWithMinAmount(receipts);
                        break;
                    
                    case ConsoleKey.Escape: // выход в главное меню
                        return;

                    default:
                        ConsoleColor oldCol = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n !!! Такого пункта меню не существует !!!");
                        Utils.Utils.WaitForKey("Чтобы повторить попытку, нажмите любую клавишу...");
                        Console.ForegroundColor = oldCol;
                        break;

                } // switch

            } // while

        } // MenuLINQ

        #endregion

        #region Вспомогательные функции

        // эмуляция прогресс-бара
        static void ProgressBar() {
            Console.Write("\n    --> [");
            ConsoleColor oldColF = Console.ForegroundColor;
            ConsoleColor oldColB = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Green;
            for (int i = 0; i < 20; i++) {
                Console.Write(" ");
                Thread.Sleep(300);
            } // for
            Console.BackgroundColor = oldColB;
            Console.Write("] <--");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\n\tБингоо! Я справился =)");
            Thread.Sleep(1500);
            Console.ForegroundColor = oldColF;
        } // ProgressBar

        // вывод всех чеков в консоль
        static void DisplayAllReceipts(List<Receipt> receipts) {
            
            // если коллекция пустая, оповещаем об этом
            if(receipts.Count == 0)
                Console.WriteLine("\n\n Коллекция чеков пуста! Пожалуйста загрузите данные. (пункт меню #6)\n\n\n");

            foreach(var item in receipts) 
                item.DisplayInfo();

            Console.WriteLine("\n\n");
            Utils.Utils.WaitForKey();
        } // DisplayAllReceipts

        // добавление товара в выбранный чек
        static void AddingGoodsInReceipts(List<Receipt> receipts) {
            try {
                Console.Write($"\nВ какой из {receipts.Count} чеков, вы хотите добавить товар?\n Номер --> ");
                int indexSelectedReceipt = Convert.ToInt32(Console.ReadLine()) - 1;

                // если введенный индекс выходит за границы коллекции - бросаем исключение
                if (indexSelectedReceipt > receipts.Count)
                    throw new IndexOutOfRangeException();

                // вывод выбранного пользователем чека
                Console.Clear();
                Console.WriteLine(" Ниже представлен выбранный Вами чек...");
                receipts[indexSelectedReceipt].DisplayInfo();

                Console.WriteLine("\n\n\t--- Заполните данные ---\n");

                // ввод кода товара
                Console.Write(" Код товара: ");
                int code = Convert.ToInt32(Console.ReadLine());

                // ввод наименования товара
                Console.Write(" Наименование товара: ");
                string name = Console.ReadLine();

                // ввод цены за 1 единицу товара
                Console.Write(" Цена за 1 единицу товара: ");
                double price = Convert.ToDouble(Console.ReadLine());

                // ввод количество покупаемых единиц товара
                Console.Write(" Количество покупаемых единиц товара: ");
                int amount = Convert.ToInt32(Console.ReadLine());

                // создаём обьект нового товара
                Goods goods = new Goods(code, name, price, amount);

                // добавление нового товара в 
                receipts[indexSelectedReceipt].AddGoods(goods);

                // оповещение пользователя о том, что товар добавлен
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n --> Товар успешно добавлен! <---");
                Console.ForegroundColor = oldCol;
            } catch (IndexOutOfRangeException) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Введенный номер превысил максимально допустимое значение!");
                Console.ForegroundColor = oldCol;
            } catch (Exception ex) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{ex.Message}");
                Console.ForegroundColor = oldCol;
            }// try/catch

            Utils.Utils.WaitForKey();
        } // AddingGoodsInReceipts

        // изменение товара в выбранном чеке
        static void UpdateGoodsInReceipt(List<Receipt> receipts) {
            try {
                Console.Write($"\nВ какой из {receipts.Count} чеков, вы хотите добавить товар?\n Номер --> ");
                int indexSelectedReceipt = Convert.ToInt32(Console.ReadLine()) - 1;

                // если введенный индекс выходит за границы коллекции - бросаем исключение
                if (indexSelectedReceipt > receipts.Count)
                    throw new IndexOutOfRangeException();

                // вывод выыбранного пользователем чека
                Console.WriteLine(" Ниже представлен выбранный Вами чек...");
                receipts[indexSelectedReceipt].DisplayInfo();

                // ввод номера изменяемого товара
                Console.Write(" Введите номер записи товара, которую хотите изменить --> ");
                int indexUpdatedGoods = Convert.ToInt32(Console.ReadLine()) - 1;

                // если индекс выходит за границы коллекции - бросаем исключение
                if (indexUpdatedGoods > receipts[indexSelectedReceipt].ListGoods.Count)
                    throw new IndexOutOfRangeException();

                // получаем товар который нужно изменить
                Goods goods = receipts[indexSelectedReceipt].ListGoods[indexUpdatedGoods];

                // меняем запись об этом товаре
                receipts[indexSelectedReceipt].UpdateGoods(goods);

                // оповещение пользователя о том, что товар изменен
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" Товар успешно изменен!");
                Console.ForegroundColor = oldCol;

            } catch (IndexOutOfRangeException) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Введенный номер превысил максимально допустимое значение!");
                Console.ForegroundColor = oldCol;

            } catch (Exception ex) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{ex.Message}");
                Console.ForegroundColor = oldCol;
            } // try/catch

            Utils.Utils.WaitForKey();
        } // UpdateGoodsInReceipt

        // удаление товара из выбранного чека
        static void DeleteGoodsFromReceipt(List<Receipt> receipts) {
            try {
                // выбор чека
                Console.Write($"\nИз какого из {receipts.Count} чеков, вы хотите удалить товар?\n Номер --> ");
                int indexSelectedReceipt = Convert.ToInt32(Console.ReadLine()) - 1;

                // если введенный индекс выходит за границы коллекции - бросаем исключение
                if (indexSelectedReceipt > receipts.Count)
                    throw new IndexOutOfRangeException();

                // вывод выыбранного пользователем чека
                Console.WriteLine(" Ниже представлен выбранный Вами чек...");
                receipts[indexSelectedReceipt].DisplayInfo();

                // ввод номера удаляемого товара
                Console.Write(" Введите номер товара, который хотите удалить --> ");
                int indexDeletedGoods = Convert.ToInt32(Console.ReadLine()) - 1;

                // если введенный индекс выходит за границы коллекции - бросаем исключение
                if (indexDeletedGoods > receipts[indexSelectedReceipt].ListGoods.Count)
                    throw new IndexOutOfRangeException();

                // получаем удаляемый товар
                Goods goodsD = receipts[indexSelectedReceipt].ListGoods[indexDeletedGoods];

                // удаляем выбранный товар
                receipts[indexSelectedReceipt].DeleteGoods(goodsD);

                // оповещение пользователя о том, что товар добавлен
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" Товар успешно удален!");
                Console.ForegroundColor = oldCol;

            } catch (IndexOutOfRangeException) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Введенный номер превысил максимально допустимое значение!");
                Console.ForegroundColor = oldCol;
            } // try/catch

            Utils.Utils.WaitForKey();
        } // DeleteGoodsFromReceipt

        // сведения о товаре по его коду
        static void InfoAboutGoodsByCode(List<Receipt> receipts) {
            try {
                Console.Write($"\nКакой из {receipts.Count} чеков, вас интересует?\n Номер --> ");
                int indexSelectedReceipt = Convert.ToInt32(Console.ReadLine()) - 1;

                // если введенный индекс выходит за границы коллекции - бросаем исключение
                if (indexSelectedReceipt > receipts.Count)
                    throw new IndexOutOfRangeException();

                // вывод выбранного пользователем чека
                Console.Clear();
                Console.WriteLine(" Ниже представлен выбранный Вами чек...");
                receipts[indexSelectedReceipt].DisplayInfo();

                Console.Write("Введите код товара : ");

                int code = Convert.ToInt32(Console.ReadLine());

                var goods1 = receipts[indexSelectedReceipt].GetGoods(code);
                if (goods1.Equals(null))
                    throw new Exception();

                Console.Clear();
                goods1.DisplayInfo();

            }
            catch (IndexOutOfRangeException) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Введенный номер превысил максимально допустимое значение!");
                Console.ForegroundColor = oldCol;
            }
            catch (Exception) {
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Данный код товара не найден =(");
                Console.ForegroundColor = oldCol;
            } // try/catch
            Utils.Utils.WaitForKey();
        } // InfoAboutGoodsByCode

        // инициализация коллекции данными
        // на случай если вдруг будут проблемы с файлом
        static void InitializeReceipts() {

            // список продуктов 1
            List<Goods> goods1 = new List<Goods> {
                new Goods(0, "Лимонад", 220.0, 2),
                new Goods(1, "Кефир", 130.0, 1),
                new Goods(2, "Йогурт", 150.0, 4),
                new Goods(3, "Молоко", 95.0, 3)
            };

            // список продуктов 2
            List<Goods> goods2 = new List<Goods> {
                new Goods(0, "Картофель", 90.0, 8),
                new Goods(1, "Свекла", 180.0, 4),
                new Goods(2, "Лук", 100.0, 4),
                new Goods(3, "Масло", 75.0, 3)
            };

            // список продуктов 3
            List<Goods> goods3 = new List<Goods> {
                new Goods(0, "Котлеты", 300.0, 11),
                new Goods(1, "Куриные крылышки", 270.0, 6),
                new Goods(2, "Куриный фарш", 230.0, 1),
                new Goods(3, "Красный перец", 60.0, 3)
            };

            // коллекция чеков
            Receipts = new List<Receipt> {
                new Receipt(1, DateTime.Now, goods1),
                new Receipt(2, DateTime.Now, goods2),
                new Receipt(3, DateTime.Now, goods3)
            };

        } // InitializeReceipts

        #endregion

    } // Program
} // ExamProject
