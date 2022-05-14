using System.Drawing;
using System.Drawing.Imaging;

namespace ColorRemover_01
{
    public partial class Form1 : Form
    {
        private FileInfo? _fileInfo = null;

        public Form1()
        {
            InitializeComponent();
            pictureBoxMain.AllowDrop = true;
        }

        private void pictureBoxMain_DragEnter(object sender, DragEventArgs e)
        {
            //コントロール内にドラッグされたとき実行される
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
                e.Effect = DragDropEffects.Copy;
            else
                //ファイル以外は受け付けない
                e.Effect = DragDropEffects.None;
        }

        private void pictureBoxMain_DragDrop(object sender, DragEventArgs e)
        {
            //コントロール内にドロップされたとき実行される
            //ドロップされたすべてのファイル名を取得する
            string[] fileNames =
                (string[])e.Data.GetData(DataFormats.FileDrop, false);

            var fileName = fileNames[0];

            OpenFile(fileName);
        }

        private void OpenFile(string fileName)
        {
            try
            {
                //ファイルを開く
                var bmp = new Bitmap(fileName);

                //カラー画像を表示する
                pictureBoxMain.Image = bmp;

                _fileInfo = new FileInfo(fileName);

                labelDescription.Visible = false;

                //モノクロ画像に差し替えて表示する
                pictureBoxMain.Image = ColorRemove(bmp, _fileInfo);

                this.Text = _fileInfo.Name;
            }
            catch (Exception ex)
            {
            }
        }

        public Bitmap ColorRemove(Bitmap img, FileInfo fileInfo)
        {
            var totleLines = img.Height;
            var percentageOld = 0;

            var result = new Bitmap(img.Width, img.Height);

            for(var y = 0; y < img.Height; y++)
            {
                for(var x = 0; x < img.Width; x++)
                {
                    var color = img.GetPixel(x, y);

                    var r = color.R;
                    var g = color.G;
                    var b = color.B;

                    byte max = r < g ? g : r;
                    max = max < b ? b : max; ;

                    var newColor = Color.FromArgb(max, max, max);

                    result.SetPixel(x, y, newColor);
                }

                var percentage = CalcPercentage(y, totleLines);
                if (percentageOld != percentage)
                {
                    percentageOld = percentage;
                    this.Text = $"{fileInfo.Name} 色抜き中 {percentage}%";
                    Application.DoEvents();
                }
            }

            return result;
        }

        public int CalcPercentage(double current, double totle)
        {
            return (int)(Math.Floor(current / totle * 100d));
        }

        private void ToolStripMenuItemSave_Click(object sender, EventArgs e)
        {
            if(_fileInfo == null) { return; }
            var path = _fileInfo.DirectoryName;
            var body = _fileInfo.FileNameBody();
            var num = 0;

            var newFileName = "";

            do
            {
                num++;
                newFileName = $"{path}\\{body}({num}).jpg";
            } while (File.Exists(newFileName));

            pictureBoxMain.Image.Save(newFileName, ImageFormat.Jpeg);
        }

        private void ToolStripMenuItemFileOpen_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter = "画像ファイル|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tiff";
            //タイトルを設定する
            ofd.Title = "開く画像ファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckPathExists = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイルを開く
                OpenFile(ofd.FileName);
            }
        }
    }

    public static class FileInfoEx
    {
        public static string FileNameBody(this FileInfo fileInfo)
        {
            var ex = fileInfo.Extension;
            var name = fileInfo.Name;
            return name.Substring(0, name.Length - ex.Length);
        }
    }
}