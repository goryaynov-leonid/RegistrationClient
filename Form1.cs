using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RegistrationClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void FileButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            filePathTextBox.Text = openFileDialog1.FileName;
        }

        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nameTextBox.Text) || string.IsNullOrEmpty(surnameTextBox.Text) ||
                string.IsNullOrEmpty(descriptionTextBox.Text) || string.IsNullOrEmpty(filePathTextBox.Text))
            {
                MessageBox.Show("Введены не все данные");
                return;
            }

            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=pictureusers;user=root;password=Root1234");

            connection.Open();

            MySqlCommand selectCurIndex = new MySqlCommand("select max(id) from users", connection);
            string select = selectCurIndex.ExecuteScalar().ToString();

            int curIndex;
            if (String.IsNullOrEmpty(select))
            {
                curIndex = 1;
            }
            else
            {
                curIndex = Convert.ToInt32(select) + 1;
            }

            MySqlCommand insertNewUser = new MySqlCommand($"insert into users (id, name, surname, description) values ({curIndex}, '{nameTextBox.Text}'," +
                $" '{surnameTextBox.Text}', '{descriptionTextBox.Text}')", connection);
            insertNewUser.ExecuteNonQuery();

            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new StringContent("newUser"), "type");
            form.Add(new StringContent(curIndex.ToString()), "index");

            var file = File.ReadAllBytes(openFileDialog1.FileName);

            form.Add(new ByteArrayContent(file, 0, file.Length), "image", openFileDialog1.FileName.Split('\\').Last());
            HttpResponseMessage response = await httpClient.PostAsync(new UriBuilder("192.168.0.89:9999").Uri, form);

            response.EnsureSuccessStatusCode();
            httpClient.Dispose();
            var sd = response.StatusCode;

            if (sd == System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show("New user registrated");
                nameTextBox.Text = "";
                surnameTextBox.Text = "";
                descriptionTextBox.Text = "";
                filePathTextBox.Text = "";
                openFileDialog1.FileName = "";
            }

        }
    }
}
