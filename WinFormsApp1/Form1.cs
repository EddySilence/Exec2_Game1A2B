using System.CodeDom;
using System.Numerics;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private GuessNumber game;
        public Form1()
        {
            InitializeComponent();
            game = new GuessNumber();
            resultLabel.Text = String.Empty;
            guessNumberButton.Enabled = false;
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            //啟動新一輪遊戲，抓取1~9四個不相同的數字
            guessNumberButton.Enabled = true;
            game.NewGame();
            resultLabel.Text = game.Hint;
        }

        private void guessNumberButton_Click(object sender, EventArgs e)
        {
            int[] inputNumber = new int[4];
            //1.抓取使用者輸入
            try
            {
                inputNumber = GetNumber();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            //2.判斷幾A幾B
            GuessResult result = game.Guess(inputNumber);
            if (result.IsSuccess == true)
            {
                MessageBox.Show("您答對了");
                guessNumberButton.Enabled = false;
            }
            else
            {
                MessageBox.Show("您答錯了");
            }

            //3.呈現資訊
            string inputAnswer = string.Empty;
            foreach (int i in inputNumber)
            {
                inputAnswer += i.ToString() + " ";
            }
            game.AddResultString($"您目前的答案{inputAnswer}", $"{result.Hint}");
            resultLabel.Text = game.Hint;
            //string inputAnswer = string.Empty;
            //foreach (int i in inputNumber)
            //{
            //    inputAnswer += i.ToString() + " ";
            //}
            //game.AddResultString($"您目前的答案{inputAnswer}", $"0 A 0 B");
            //resultLabel.Text = game.Hint;

        }
        /// <summary>
        /// 取得使用者輸入
        /// </summary>
        /// <returns>回傳四個不相同的數字</returns>
        /// <exception cref="Exception"></exception>
        private int[] GetNumber()
        {
            string input = inputTextBox.Text;
            int[] arInputNumber = new int[4];
            char[] originalArNumber = input.ToCharArray();
            char[] arNumber = input.ToCharArray();//將字串轉成字元
            Array.Sort(arNumber);//排序

            //if (string.IsNullOrEmpty(input)) throw new Exception("請輸入數字");
            bool isInt = int.TryParse(input, out int number);//判斷有沒有轉成功
            if (input.Length != 4) throw new Exception("請輸入四個數字");//如果不為四個數字，拋出例外
            if (!isInt) throw new Exception("請輸入數字");//如果轉失敗，拋出例外
            //判斷使用者輸入的數字，有沒有一樣的數字，有就拋出例外
            for (int i = 1; i < input.Length; i++)
            {
                if (arNumber[i] == arNumber[i - 1])
                {
                    throw new Exception("數字有重複，請重新輸入謝謝");
                }
            }
            //把字串存入陣列裡面
            for (int i = 0; i < originalArNumber.Length; i++)
            {
                arInputNumber[i] = Convert.ToInt32(originalArNumber[i]) - 48;
            }
            //foreach (int i in arNumber)
            //{
            //    arInputNumber[i] = Convert.ToInt32(arNumber[i]) + 48;
            //}

            //如果上面判斷完沒問題，就回傳目前使用者輸入的值(陣列)
            return arInputNumber;
        }
    }

    public class GuessNumber
    {
        public int[] NumberAnswer = new int[4];//答案
        public string result { get; set; }
        public void NewGame()
        {
            result = string.Empty;
            int seed = Guid.NewGuid().GetHashCode();//亂數種子
            Random random = new Random(seed);
            //取四個不相同的數字
            for (int i = 0; i <= 3; i++)
            {
                int randomNumber = random.Next(1, 10);//1~10取亂數
                this.NumberAnswer[i] = randomNumber;
                for (int j = 0; j <= i - 1; j++)
                {
                    while (this.NumberAnswer[i] == this.NumberAnswer[j])
                    {
                        j = 0;
                        this.NumberAnswer[i] = random.Next(1, 10);
                    }
                }
            }
            string correctAnswer = string.Empty;
            foreach (int i in this.NumberAnswer)
            {
                correctAnswer += i.ToString() + " ";
            }
            AddResultString($"正確的答案是{correctAnswer}", "0 A 0 B");
        }

        public string Hint
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }

        public string AddResultString(string first, string second)
        {
            return result += $"{first}            目前是{second}\r\n";
        }

        public GuessResult Guess(int[] number)
        {
            //判斷幾A幾B
            int bingoCountA = 0;
            int bingoCountB = 0;
            for (int i = 0; i < number.Length; i++)
            {
                if (NumberAnswer[i] == number[i])//如果一樣就是A++
                {
                    bingoCountA++;
                }
                else//不一樣就比較全部，如果有一樣B++
                {
                    for (int j = 0; j < number.Length; j++)
                    {
                        if (NumberAnswer[i] == number[j])
                        {
                            bingoCountB++;
                        }
                    }
                }
            }
            //foreach (var item in number)
            //{
            //    if (NumberAnswer[item] == number[item])
            //    {
            //        bingoCount++;
            //    }
            //}
            if (bingoCountA == 4)//答對了
            {
                return GuessResult.Success($"{bingoCountA} A {bingoCountB} B\r\n恭喜答對了");
            }
            else//答錯了
            {
                return GuessResult.Failed($"{bingoCountA} A {bingoCountB} B");
            }

        }
    }
    public class GuessResult
    {
        public bool IsSuccess { get; set; }
        public string Hint { get; set; }
        public static GuessResult Success(string sucessMessage)
        {
            return new GuessResult { IsSuccess = true, Hint = sucessMessage };
        }
        public static GuessResult Failed(string errMessage)
        {
            return new GuessResult { IsSuccess = false, Hint = errMessage };
        }
        public bool IsFailed
        {
            get
            {
                return !IsSuccess;
            }
        }
    }

}