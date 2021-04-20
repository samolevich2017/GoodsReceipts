using System;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace ExamProject {

    // класс представляет - Товар
    [DataContract]
    class Goods {

        // код товара
        [DataMember]
        private int _codeG;
        public int CodeG {
            get => _codeG;
            set {
                if (value < 0)
                    MessageBox.Show("Goods: Недопустимое значение кода товара!", "Ошибка!", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    _codeG = value;
            } // set
        } // CodeG

        // наименование товара
        [DataMember]
        private string _nameG;
        public string NameG {
            get => _nameG;
            set {
                if (string.IsNullOrWhiteSpace(value))
                    MessageBox.Show("Goods: Наименование товара не может быть пустым!", "Ошибка!",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    _nameG = value;
            } // set
        } // NameG

        // цена за единицу товара
        [DataMember]
        private double _priceG;
        public double PriceG {
            get => _priceG;
            set {
                if (value <= 0)
                    MessageBox.Show("Goods: Цена за единицу товара не может быть <= 0!", "Ошибка!",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    _priceG = value;
            } // set
        } // PriceG

        // количество покупаемых единиц товара
        [DataMember]
        private int _amountG;
        public int AmountG {
            get => _amountG;
            set {
                if (value < 0)
                    MessageBox.Show("Goods: Количество покупаемых единиц товара не может быть < 0!", "Ошибка!",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    _amountG = value;
            } // set
        } // AmountG

        // конструктор по умолчанию
        public Goods():this(0, "Отсутствует", 0.0d, 0) { }

        // конструктор с параметрами
        public Goods(int code, string name, double price, int amount) {
            CodeG = code;
            NameG = name;
            PriceG = price;
            AmountG = amount;
        } // Goods

        // вычисление суммы оплаты за товар
        public double AmountOfPaymentForTheGoods() {
            return PriceG * AmountG;
        } // AmountOfPaymentForTheGood

        // вывод товара
        public void DisplayInfo() {
            Console.WriteLine(" +-------------------------+");
            Console.WriteLine("\t    ТОВАР");
            Console.WriteLine(" +-------------------------+");
            Console.WriteLine($"   Код : {CodeG}");
            Console.WriteLine($"   Наим.: {NameG}");
            Console.WriteLine($"   Цена : {PriceG:N2} руб.");
            Console.WriteLine($"   Количество : {AmountG}");
            Console.WriteLine(" +-------------------------+\n");
        } // DisplayInfo

        // строковое представление обьекта
        public override string ToString() {
            return $"Код товара : {_codeG}\nНаименование товара : {NameG}\nЦена за 1-ед. товара : {_priceG:N2}\nКол-во покупаемых единиц : {_amountG}";
        } // ToString

    } // Goods
} // ExamProject
