using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ece327lab3helper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Amadeus Wieczorek 2012
    /// </summary>
    public partial class MainWindow : Window
    {
        private int[,] inputArray;
        private int[,] resultArray;
        private int[,] countSoFarArray;
        private int positivesCount;
        private int currentRow;
        private int currentColumn;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadTestData(String fileName)
        {
            // Cleanup data
            positivesCount = 0;
            inputArray = new int[16,16];
            resultArray = new int[16,16];
            countSoFarArray = new int[16,16];
            currentColumn = 0;
            currentRow = 0;

            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line;
                    currentRow = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            String[] dataBits = line.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                            currentColumn = 0;
                            foreach (string dataBit in dataBits)
                            {
                                inputArray[currentRow, currentColumn] = Convert.ToInt32(dataBit);
                                currentColumn++;
                            }
                            currentRow++;

                        }
                        catch (Exception e)
                        {
                            throw new Exception("Could not process file contents: " + e.ToString(), e);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                throw new Exception("Could not read file: " + e.ToString(), e);
            }


            // Now calculate the results
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (i >= 2)
                    {
                        resultArray[i, j] = inputArray[i - 2, j] - inputArray[i - 1, j] + inputArray[i, j];
                        if (resultArray[i, j] >= 0)
                        {
                            positivesCount++;
                        }
                        countSoFarArray[i, j] = positivesCount;
                    }
                    else
                    {
                        resultArray[i, j] = 0;
                    }
                }
            }


            // Set up the environment
            currentColumn = 0;
            currentRow = 2;

            redrawScreen();

        }


        private void redrawScreen()
        {
            resultBlock1.Text = "Count = " + positivesCount.ToString();

            // Fill in the input RTF box
            fillRTF(inputFileBox, inputArray, 3);
            fillRTF(resultBox, resultArray, 1);
            fillRTF(countSoFarBox, countSoFarArray, 1);

        }

        private void fillRTF(RichTextBox box, int[,] dataArray, int selectMode)
        {
            FlowDocument displayedText = new FlowDocument();
            for (int i = 0; i < 16; i++)
            {
                Paragraph currentLine = new Paragraph();

                if (i <= currentRow && i >= currentRow - (selectMode-1))
                {
                    Run currentLineStart = new Run();
                    for (int column = 0; column < currentColumn; column++)
                    {
                        currentLineStart.Text += String.Format("{0,4}", dataArray[i, column]);
                    }

                    Bold currentElement = new Bold();
                    currentElement.Inlines.Add(String.Format("{0,4}", dataArray[i, currentColumn]));
                    if (i == currentRow)
                    {
                        currentElement.Background = new SolidColorBrush(Colors.DarkOrange);
                    }
                    else
                    {
                        currentElement.Background = new SolidColorBrush(Colors.DarkMagenta);
                    }

                    Run currentLineEnd = new Run();
                    for (int column = currentColumn + 1; column < 16; column++)
                    {
                        currentLineEnd.Text += String.Format("{0,4}", dataArray[i, column]);
                    }

                    /*
                                    Style smallParagraphs = new Style(typeof(Paragraph));
                                    smallParagraphs.Setters.Add(new Setter
                                    {
                                        Property = Paragraph.TextIndentProperty,
                                        Value = 30.0
                                    });

                                    currentLine.Style = smallParagraphs;
                     */
                    currentLine.Inlines.Add(currentLineStart);
                    currentLine.Inlines.Add(currentElement);
                    currentLine.Inlines.Add(currentLineEnd);
                }
                else
                {
                    Run currentLineStart = new Run();
                    for (int column = 0; column < 16; column++)
                    {
                        currentLineStart.Text += String.Format("{0,4}", dataArray[i, column]);
                    }
                    currentLine.Inlines.Add(currentLineStart);
                }
                displayedText.Blocks.Add(currentLine);
            }
            box.Document = displayedText;
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            //loadTestData(@"C:\Users\Amadeus\Documents\Visual Studio 2010\Projects\ece327lab3helper\ece327lab3helper\bin\Debug\test1.txt");
            try
            {
                loadTestData(String.Format("test{0}.txt", (sender as Button).Tag));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load file test" + (sender as Button).Tag + ".txt \n Exception: " +ex);
            }
            
        }

        private void inputFileBox_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void inputFileBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextPointer tp = (sender as RichTextBox).CaretPosition;

           /* int index = 0;
            TextPointer pos = (sender as RichTextBox).;
            while (pos.CompareTo(tp) != 0)
            {
                if (pos.IsAtInsertionPosition)
                {
                    index++;
                }

                pos = pos.GetNextInsertionPosition(LogicalDirection.Forward);
            }*/
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    IncrementColumn();
                    redrawScreen();
                    break;
                case Key.Left:
                    DecrementColumn();
                    redrawScreen();
                    break;
                case Key.Down:
                    IncrementRow();
                    redrawScreen();
                    break;
                case Key.Up:
                    DecrementRow();
                    redrawScreen();
                    break;
            }
        }

        private bool IncrementRow()
        {
            if (currentRow == 15)
            {
                return false;
            }
            else
            {
                currentRow++;
                return true;
            }
        }

        private bool DecrementRow()
        {
            if (currentRow == 0)
            {
                return false;
            }
            else
            {
                currentRow--;
                return true;
            }
        }

        private bool IncrementColumn()
        {
            if (currentColumn == 15)
            {
                if (IncrementRow())
                {
                    currentColumn = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                currentColumn++;
                return true;
            }
        }

        private bool DecrementColumn()
        {
            if (currentColumn == 0)
            {
                if (DecrementRow())
                {
                    currentColumn = 15;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                currentColumn--;
                return true;
            }
        }

    }
}
