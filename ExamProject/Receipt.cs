using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace ExamProject {

    // класс представляет - Чек
    [DataContract]
    class Receipt {

        // номер товарного чека
        [DataMember]
        private int _numberR;
        public int NumberR {
            get => _numberR;
            set {
                if (value < 0)
                    MessageBox.Show("Receipt: Номер товарного чека не корректен!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    _numberR = value;
            } // set
        } // NumberR

        // дата и время создания чека
        [DataMember]
        public DateTime DTCreated { get; set; } 

        // список покупаемых товаров
        [DataMember]
        public List<Goods> ListGoods { get; set; }

        // конструктор по умолчанию
        public Receipt() { }

        // конструктор с параметрами
        public Receipt(int number, DateTime dateTime, List<Goods> goods) {
            NumberR = number;
            DTCreated = dateTime;
            ListGoods = goods;
        } // Receipt

        // добавление записи (товара)
        public void AddGoods(Goods goods) {
            ListGoods.Add(goods);
        } // AddRecord

        // удаление записи (товара)
        public void DeleteGoods(Goods goods) {

            // получаем индекс указанного обьекта (товара)
            int index = ListGoods.IndexOf(goods);

            // удаляем его из коллекции по полученному индексу
            ListGoods.RemoveAt(index);

        } // DeleteRecord

        // изменение записи (товара)
        public void UpdateGoods(Goods goods) {
            
            // получаем индекс указанного обьекта (товара)
            int index = ListGoods.IndexOf(goods);

            Console.WriteLine($" ------------- Изменение записи №{index + 1} ------------- ");

            // меняем данные обьекта (товара)
            Console.WriteLine("Пожалуйста, заполните данные ниже...");
            Console.Write("Код товара : ");
            ListGoods[index].CodeG = Convert.ToInt32(Console.ReadLine()); // код товара
            Console.Write("Наименование товара : ");
            ListGoods[index].NameG = Console.ReadLine(); // наименование товара
            Console.Write("Цена товара : ");
            ListGoods[index].PriceG = Convert.ToDouble(Console.ReadLine()); // цена товара
            Console.Write("Количество : ");
            ListGoods[index].AmountG = Convert.ToInt32(Console.ReadLine()); // кол-во покупаемых ед. товара

        } // UpdateGoods

        // сведения о товаре по его коду
        public Goods GetGoods(int code) {
            Goods res = ListGoods.Where(g => g.CodeG == code).FirstOrDefault();
            return res;
        } // GetGoods

        // подсчёт общей суммы покупки
        public double TotalSum() {

            double totalS = 0;
            foreach (var item in ListGoods)
                totalS += item.AmountOfPaymentForTheGoods();

            return totalS;
        } // TotalSum

        // вывод информации о чеках
        public void DisplayInfo() {
            // TODO: View dateTime

            string numRec = $"  / Чек #{NumberR} \\ ";
            Console.WriteLine(" -------------------------------");
            Console.WriteLine($"           Чек №___{NumberR}");
            Console.WriteLine($"     * {DTCreated} *");
            Console.Write(" -------------------------------\n");

            // вывод списка продуктов
            int count = 1;
            foreach (var item in ListGoods) {
                Console.Write($"  {count++}."); 
                ConsoleColor oldCol = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"Код[{item.CodeG}]-");
                Console.ForegroundColor = oldCol;
                Console.Write($"{ item.NameG}");
                Console.Write($" {item.AmountG}x{item.PriceG:N2}\n");
                Console.WriteLine($"   = {item.AmountOfPaymentForTheGoods():N2} руб. ");
            } // foreach

           Console.WriteLine($" -------------------------------\n      Итого: {TotalSum():N2} руб.");
           Console.WriteLine(" -------------------------------");
        } // DisplayInfo

    } // Receip
} // ExamProject
