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
            //�Ұʷs�@���C���A���1~9�|�Ӥ��ۦP���Ʀr
            guessNumberButton.Enabled = true;
            game.NewGame();
            resultLabel.Text = game.Hint;
        }

        private void guessNumberButton_Click(object sender, EventArgs e)
        {
            int[] inputNumber = new int[4];
            //1.����ϥΪ̿�J
            try
            {
                inputNumber = GetNumber();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            //2.�P�_�XA�XB
            GuessResult result = game.Guess(inputNumber);
            if (result.IsSuccess == true)
            {
                MessageBox.Show("�z����F");
                guessNumberButton.Enabled = false;
            }
            else
            {
                MessageBox.Show("�z�����F");
            }

            //3.�e�{��T
            string inputAnswer = string.Empty;
            foreach (int i in inputNumber)
            {
                inputAnswer += i.ToString() + " ";
            }
            game.AddResultString($"�z�ثe������{inputAnswer}", $"{result.Hint}");
            resultLabel.Text = game.Hint;
            //string inputAnswer = string.Empty;
            //foreach (int i in inputNumber)
            //{
            //    inputAnswer += i.ToString() + " ";
            //}
            //game.AddResultString($"�z�ثe������{inputAnswer}", $"0 A 0 B");
            //resultLabel.Text = game.Hint;

        }
        /// <summary>
        /// ���o�ϥΪ̿�J
        /// </summary>
        /// <returns>�^�ǥ|�Ӥ��ۦP���Ʀr</returns>
        /// <exception cref="Exception"></exception>
        private int[] GetNumber()
        {
            string input = inputTextBox.Text;
            int[] arInputNumber = new int[4];
            char[] originalArNumber = input.ToCharArray();
            char[] arNumber = input.ToCharArray();//�N�r���ন�r��
            Array.Sort(arNumber);//�Ƨ�

            //if (string.IsNullOrEmpty(input)) throw new Exception("�п�J�Ʀr");
            bool isInt = int.TryParse(input, out int number);//�P�_���S���ন�\
            if (input.Length != 4) throw new Exception("�п�J�|�ӼƦr");//�p�G�����|�ӼƦr�A�ߥX�ҥ~
            if (!isInt) throw new Exception("�п�J�Ʀr");//�p�G�ॢ�ѡA�ߥX�ҥ~
            //�P�_�ϥΪ̿�J���Ʀr�A���S���@�˪��Ʀr�A���N�ߥX�ҥ~
            for (int i = 1; i < input.Length; i++)
            {
                if (arNumber[i] == arNumber[i - 1])
                {
                    throw new Exception("�Ʀr�����ơA�Э��s��J����");
                }
            }
            //��r��s�J�}�C�̭�
            for (int i = 0; i < originalArNumber.Length; i++)
            {
                arInputNumber[i] = Convert.ToInt32(originalArNumber[i]) - 48;
            }
            //foreach (int i in arNumber)
            //{
            //    arInputNumber[i] = Convert.ToInt32(arNumber[i]) + 48;
            //}

            //�p�G�W���P�_���S���D�A�N�^�ǥثe�ϥΪ̿�J����(�}�C)
            return arInputNumber;
        }
    }

    public class GuessNumber
    {
        public int[] NumberAnswer = new int[4];//����
        public string result { get; set; }
        public void NewGame()
        {
            result = string.Empty;
            int seed = Guid.NewGuid().GetHashCode();//�üƺؤl
            Random random = new Random(seed);
            //���|�Ӥ��ۦP���Ʀr
            for (int i = 0; i <= 3; i++)
            {
                int randomNumber = random.Next(1, 10);//1~10���ü�
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
            AddResultString($"���T�����׬O{correctAnswer}", "0 A 0 B");
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
            return result += $"{first}            �ثe�O{second}\r\n";
        }

        public GuessResult Guess(int[] number)
        {
            //�P�_�XA�XB
            int bingoCountA = 0;
            int bingoCountB = 0;
            for (int i = 0; i < number.Length; i++)
            {
                if (NumberAnswer[i] == number[i])//�p�G�@�˴N�OA++
                {
                    bingoCountA++;
                }
                else//���@�˴N��������A�p�G���@��B++
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
            if (bingoCountA == 4)//����F
            {
                return GuessResult.Success($"{bingoCountA} A {bingoCountB} B\r\n���ߵ���F");
            }
            else//�����F
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