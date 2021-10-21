using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Laba_1
{
    internal sealed class Program
    {
        //Большие буквы английского алфавита
        private const string BIG_EN = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //Маленькие буквы английского алфавита
        private static readonly string SMALL_EN = BIG_EN.ToLower();
        //нижняя граница диапозона ключа
        private const int LOWER_BOUND = -10;
        //верхняя граница диапозона ключа
        private const int UPPER_BOUND = 0;
        //поле типа Цезарь
        private static Cezar cezar;
        static void Main(string[] args)
        {
            //Инициализируем экзмемпляр класса шифра Цезаря, передаем элементы алфавита и диапозон ключа согласно варианту
            InicializeCezar();
            while(true)
            {
                //печатаем меню комманд
                printMenu();
                int command;
                //вводим номер нужной команды. Если введено не число, TryParse не передаст значение по ссылке в переменную command
                Int32.TryParse(Console.ReadLine(), out command);
                switch (command)
                {
                   
                    //шифруем текст
                    case 1:
                        //если введенный ключ не соответствует диапозону,вводим повторно
                        while (true)
                        {
                            Console.WriteLine("Введите ключ в диапозоне [{0};{1}]", LOWER_BOUND, UPPER_BOUND);
                            if (EnterKey(Console.ReadLine())) break;
                        }
                        
                        Console.WriteLine("Укажите путь к  открытому текст");
                        string filePath = Console.ReadLine();
                        string openText = File.ReadAllText(filePath);
                        //передаем методу CryptText открытый текст, который возвращает зашифрованный и записываем изменения в этот файл
                        File.WriteAllText(filePath, cezar.CryptText(openText));
                        //открываем файл для проверки выполненного шифрования
                        Process.Start(filePath);
                        break;                   
                   //дешифруем текст
                    case 2:
                        //если введенный ключ не соответствует диапозону,вводим повторно
                        while (true)
                        {
                            Console.WriteLine("Введите ключ в диапозоне [{0};{1}]", LOWER_BOUND, UPPER_BOUND);
                            if (EnterKey(Console.ReadLine())) break;
                        }

                        Console.WriteLine("Укажите путь к  зашифрованному текст");
                        string pathFile = Console.ReadLine();
                        string encryptedText = File.ReadAllText(pathFile);
                        //передаем методу DecryptText зашифрованный текст, который возвращает дешифрованный и записываем изменения в этот файл
                        File.WriteAllText(pathFile, cezar.DecryptText(encryptedText));
                        //открываем файл для проверки выполненного дешифрования
                        Process.Start(pathFile);
                        break;
                   //частотный криптоанализ буквы
                    case 3:
                        Console.WriteLine("Укажите путь к зашифрованному текст");
                        string encryptText = File.ReadAllText(Console.ReadLine());
                        Console.WriteLine("Укажите путь к эталонному тексту");
                        string referenceText = File.ReadAllText(Console.ReadLine());
                        //вызываем метод,возвращающий возможный ключ.
                        Console.WriteLine("Возможный ключ = {0} в диапозоне [{1},{2}]",
                            cezar.TryCrackCezar(encryptText,referenceText)
                            ,LOWER_BOUND,
                            UPPER_BOUND);
                        break;
                    //выход из меню
                    case 0:
                        return;
                }
            }
            

        }
        private static bool EnterKey(string enteredKey)
        {
            //если  получилось установить введенный ключ из диапозона,выходим из блока 
            if (cezar.SetKey(Int32.Parse(enteredKey)))
            {
                return true;
            }
            //если метод SetKey не установил ключ,возвоащаем false
            else 
            {
                Console.WriteLine("Ключ не принадлежит диапозону! [{0};{1}]", LOWER_BOUND, UPPER_BOUND);
                return false;
            }
        }
        private static void printMenu()
        {
            Console.WriteLine(
                " \n Выберите операцию:" +
                " \n 1. Зашифровать сообщение. " +
                " \n 2. Дешифровать сообщение." +
                " \n 3. Криптоанализ текста" +
                " \n 0. Выход");
        }
        private static void InicializeCezar()
        {
               cezar = new Cezar(
               BIG_EN + SMALL_EN,
               LOWER_BOUND,
               UPPER_BOUND);
        }
    }

    internal class Cezar
    {
        private readonly string _alphabet;
        private readonly int _lowerBound;
        private readonly int _upperBound;
        private readonly int _alphLen;
        private int _key;

        public Cezar(string alphabet, 
            int lowerBoundKey,
            int upperBoundKey)
        {
            _alphabet = alphabet;
            _alphLen = alphabet.Length;
            _lowerBound = lowerBoundKey;
            _upperBound = upperBoundKey;
        }
        public bool SetKey(int enteredKey)
        {
            if (CheckCorrectKey(enteredKey))
            {
                _key = ConvertKeyFromRange(enteredKey);
                return true;
            }
            else return false;
        }
        private bool CheckCorrectKey(int enteredKey)
        {
            if (enteredKey >= _lowerBound &&
                enteredKey <= _upperBound) return true;
            else return false;
        }
        private int ConvertKeyFromRange(int keyFromRange)
        {
            if (keyFromRange < 0) keyFromRange += _alphLen;
            else if (keyFromRange >= _alphLen) keyFromRange -= _alphLen;

            return keyFromRange;
        }
        internal string CryptText(string text)
        {
            string encryptedText = String.Empty;
            //перебираем все символы в тексте
            for (int i = 0; i < text.Length; i++)
            {
               //если текущий символ текста есть в алфавите
                if (_alphabet.Contains(text[i]))
                { 
                   //вычисляем индекс в алфавите для зашифрованного символа по формуле
                    int x = _alphabet.IndexOf(text[i]);
                    int y = (x + _key) % _alphLen;
                    //находим элемент алфавита по вычисленному индексу выше и записываем в итоговую строку
                    encryptedText += _alphabet[y];
                }
                //если текущий символ не принадлежит алфавиту
                else encryptedText += text[i];
            }
            return encryptedText;
        }
        internal string DecryptText(string text)
        {
            string decryptedText = String.Empty;
            //перебираем все символы в тексте
            for (int i = 0; i < text.Length; i++)
            {
                //если текущий символ зашифрованного текста есть в алфавите
                if (_alphabet.Contains(text[i]))
                {
                    //вычисляем индекс в алфавите для зашифрованного символа по формуле
                    int y = _alphabet.IndexOf(text[i]);
                    int x = (y - _key + +_alphLen) % _alphLen;
                    //находим элемент алфавита по вычисленному индексу выше и записываем в итоговую строку
                    decryptedText += _alphabet[x];
                }
                //если текущий символ не принадлежит алфавиту
                else decryptedText += text[i];
            }
            return decryptedText;
        }
        private int[] GetFrequencyArray(string text)
        {
            int[] frequencys = new int[_alphLen];
           //перебираем все элементы текста
            for(int i =0;i < text.Length;i++)
            {
                //проверяем, является ли текущий элемент текста элементом алфавита,и если да,
                //то прибавляем 1 к элемету по индексу j частотного массива,где j-соответствующий индекс элемента в алфавите
                for (int j = 0; j < _alphLen; j++)
                {
                    if (text[i] == _alphabet[j])
                    {
                        frequencys[j]++;
                        break;
                    }
                }
            }
            return frequencys;
        }
        private int GetIndexOfMaxValue(int[] frequencys)
        {
          //Метод Max возвращает максимальное значение в массиве частот встречаемости элементов алфавита
            int maxValue = frequencys.Max();
            int maxIndex = 0;
            //перебирая все элементы массива частот,определяем через равенство индекс максимально встречающегося элемента алфавита
            for (int i = 0; i < frequencys.Length; i++)
                if (frequencys[i] == maxValue)
                {
                    maxIndex = i;
                    break;
                }
            return maxIndex;
        }
        public int TryCrackCezar(string encryptedText, string referenceText)
        {
            int possibleKey;
            //вызываем метод, определяющий сколько раз встречается каждая букву,принадлежащая заданному алфавиту из зашифрованного текста
            int[] frequencyEncrypt = GetFrequencyArray(encryptedText);
            //вызываем метод, определяющий сколько раз встречается каждая букву,принадлежащая заданному алфавиту из эталонного текста
            int[] frequencyReference = GetFrequencyArray(referenceText);
            //определяем индекс элемента алфавита, больше всего встречающегося в зашифрованном тексте
            int indexOfMaxFreqInEncr = GetIndexOfMaxValue(frequencyEncrypt);
            int indexOfMaxFreqInRef = GetIndexOfMaxValue(frequencyReference);
            //предполагая, что максимально встречающийся элемент алфавита в зашифрованном тексте
            //соответствует максимально встречающемуся элементу алфавита в эталонном тексте
            //находим возможный ключ как разность индексов этих элементов в алфавите
            possibleKey = indexOfMaxFreqInEncr - indexOfMaxFreqInRef;
            //если разность оказалась меньше 0, то прибавляем значение мощности алфавита
            //if (possibleKey < 0) possibleKey += _alphLen;
            return possibleKey;
        }
    }
}
