using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace _2305222
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        MySqlConnection sqlConnect = new MySqlConnection("server=localhost;uid=root;pwd=;database=premier_league");
        MySqlCommand sqlCommand;
        MySqlDataAdapter sqlAdapter;
        String sqlQuery;
        DataTable dtPlayer = new DataTable();
        DataTable dtNationality = new DataTable();
        DataTable dtTeamName = new DataTable();
        DataTable dtTeamNumber = new DataTable();
        DataTable dtCapt = new DataTable();
        DataTable dtSaveNation = new DataTable();
        DataTable dtSaveTeam = new DataTable();
        string tim;
        int posisiIndex;
        int cek = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlQuery = "select p.player_id, p.player_name, p.birthdate, n.nation, t.team_name, p.team_number from player p left join team t on p.team_id = t.team_id left join nationality n on p.nationality_id = n.nationality_id;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtPlayer);
            sqlQuery = "select nation from nationality; ";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtNationality);
            sqlQuery = "select team_name from team ";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtTeamName);

            for (int i = 0; i< dtNationality.Rows.Count; i++)
            {
                cbNationality.Items.Add(dtNationality.Rows[i][0].ToString());
            }
            for (int i = 0; i<dtTeamName.Rows.Count; i++)
            {
                cbTeam.Items.Add(dtTeamName.Rows[i][0].ToString());
            }

            dataPlayer(0);
        }

        public void dataPlayer (int Posisi)
        {
            tbID.Text = dtPlayer.Rows[Posisi][0].ToString();
            tbName.Text = dtPlayer.Rows[Posisi][1].ToString();
            dateBirth.Text = dtPlayer.Rows[Posisi][2].ToString();
            cbNationality.SelectedIndex = cbNationality.FindStringExact(dtPlayer.Rows[Posisi][3].ToString());
            cbTeam.SelectedIndex = cbTeam.FindStringExact(dtPlayer.Rows[Posisi][4].ToString());
            nNumber.Value = Convert.ToInt32(dtPlayer.Rows[Posisi][5].ToString());
            posisiIndex = Posisi;
            tim = dtPlayer.Rows[Posisi][4].ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cek == 0)
            {
                dtSaveTeam.Clear();
                dtSaveNation.Clear();
                sqlQuery = "select nationality_id from nationality where nation = '"+ cbNationality.Text +"';";
                sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
                sqlAdapter = new MySqlDataAdapter(sqlCommand);
                sqlAdapter.Fill(dtSaveNation);
                sqlQuery = "select team_id from team where team_name = '"+ cbTeam.Text +"';";
                sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
                sqlAdapter = new MySqlDataAdapter(sqlCommand);
                sqlAdapter.Fill(dtSaveTeam);

                sqlQuery = "update player set player_name = '"+ tbName.Text.ToString() +"', birthdate = '"+ dateBirth.Value.ToString("yyyMMdd") +"', nationality_id = '"+ dtSaveNation.Rows[0][0].ToString() + "', team_id = '"+ dtSaveTeam.Rows[0][0].ToString() + "', team_number = '"+ nNumber.Value.ToString() +"' where player_id = '"+ tbID.Text.ToString() +"';";
                sqlConnect.Open();
                sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
                sqlAdapter = new MySqlDataAdapter(sqlCommand);
                sqlCommand.ExecuteNonQuery();
                sqlConnect.Close();

                sqlQuery = "select captain_id from team where team_name = '"+ tim +"';";
                sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
                sqlAdapter = new MySqlDataAdapter(sqlCommand);
                sqlAdapter.Fill(dtCapt);

                if (dtCapt.Rows[0][0].ToString() == tbID.Text)
                {
                    sqlQuery = "update team set captain_id = (select p.player_id from player p left join team t on p.team_id = t.team_id where t.team_name = '" + tim + "' order by p.birthdate;) where team_name = '" + tim + "'";
                    sqlConnect.Open();
                    sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
                    sqlAdapter = new MySqlDataAdapter(sqlCommand);
                    sqlCommand.ExecuteNonQuery();
                    sqlConnect.Close();
                }

                dtPlayer.Clear();
                sqlQuery = "select p.player_id, p.player_name, p.birthdate, n.nation, t.team_name, p.team_number from player p left join team t on p.team_id = t.team_id left join nationality n on p.nationality_id = n.nationality_id";
                sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
                sqlAdapter = new MySqlDataAdapter(sqlCommand);
                sqlAdapter.Fill(dtPlayer);

                dataPlayer(posisiIndex);

                MessageBox.Show("Data Sudah Tersimpan");
            }
            else
                MessageBox.Show("Not Available");
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dtTeamNumber.Clear();
            sqlQuery = "select p.team_number from player p left join team t on p.team_id = t.team_id where t.team_name = '"+ dtPlayer.Rows[posisiIndex][4].ToString() + "' and p.team_number != "+ Convert.ToInt32(dtPlayer.Rows[posisiIndex][5].ToString()) + " order by 1";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtTeamNumber);
            cek = 0;
            for (int i = 0; i < dtTeamNumber.Rows.Count; i++)
            {
                if (Convert.ToInt32(dtTeamNumber.Rows[i][0]) == Convert.ToInt32(nNumber.Value))
                {
                    cek = 1;
                }
            }
            if (cek == 1)
                lbAva.Text = "Not Available";
            else
                lbAva.Text = "Available";
        }
        private void btnFirst_Click(object sender, EventArgs e)
        {
            dataPlayer(0);
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (posisiIndex > 0)
            {
                posisiIndex--;
                dataPlayer(posisiIndex);
            }
            else
                MessageBox.Show("Data Sudah Data Pertama");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (posisiIndex < dtPlayer.Rows.Count - 1)
            {
                posisiIndex++;
                dataPlayer(posisiIndex);
            }
            else
                MessageBox.Show("Data Sudah Data Terakhir");
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            dataPlayer(dtPlayer.Rows.Count - 1);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
