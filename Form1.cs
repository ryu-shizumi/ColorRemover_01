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
            //�R���g���[�����Ƀh���b�O���ꂽ�Ƃ����s�����
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                //�h���b�O���ꂽ�f�[�^�`���𒲂ׁA�t�@�C���̂Ƃ��̓R�s�[�Ƃ���
                e.Effect = DragDropEffects.Copy;
            else
                //�t�@�C���ȊO�͎󂯕t���Ȃ�
                e.Effect = DragDropEffects.None;
        }

        private void pictureBoxMain_DragDrop(object sender, DragEventArgs e)
        {
            //�R���g���[�����Ƀh���b�v���ꂽ�Ƃ����s�����
            //�h���b�v���ꂽ���ׂẴt�@�C�������擾����
            string[] fileNames =
                (string[])e.Data.GetData(DataFormats.FileDrop, false);

            var fileName = fileNames[0];

            OpenFile(fileName);
        }

        private void OpenFile(string fileName)
        {
            try
            {
                //�t�@�C�����J��
                var bmp = new Bitmap(fileName);

                //�J���[�摜��\������
                pictureBoxMain.Image = bmp;

                _fileInfo = new FileInfo(fileName);

                labelDescription.Visible = false;

                //���m�N���摜�ɍ����ւ��ĕ\������
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
                    this.Text = $"{fileInfo.Name} �F������ {percentage}%";
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
            //OpenFileDialog�N���X�̃C���X�^���X���쐬
            OpenFileDialog ofd = new OpenFileDialog();

            //[�t�@�C���̎��]�ɕ\�������I�������w�肷��
            //�w�肵�Ȃ��Ƃ��ׂẴt�@�C�����\�������
            ofd.Filter = "�摜�t�@�C��|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tiff";
            //�^�C�g����ݒ肷��
            ofd.Title = "�J���摜�t�@�C����I�����Ă�������";
            //�_�C�A���O�{�b�N�X�����O�Ɍ��݂̃f�B���N�g���𕜌�����悤�ɂ���
            ofd.RestoreDirectory = true;
            //���݂��Ȃ��t�@�C���̖��O���w�肳�ꂽ�Ƃ��x����\������
            //�f�t�H���g��True�Ȃ̂Ŏw�肷��K�v�͂Ȃ�
            ofd.CheckFileExists = true;
            //���݂��Ȃ��p�X���w�肳�ꂽ�Ƃ��x����\������
            //�f�t�H���g��True�Ȃ̂Ŏw�肷��K�v�͂Ȃ�
            ofd.CheckPathExists = true;

            //�_�C�A���O��\������
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //OK�{�^�����N���b�N���ꂽ�Ƃ��A�I�����ꂽ�t�@�C�����J��
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