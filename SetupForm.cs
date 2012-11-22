using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectAlphaIota
{
    public partial class SetupForm : Form
    {
        RadioButton _vsType;
        RadioButton _playerColor;
        private CheckerGame game;
        public SetupForm(CheckerGame game)
        {
            this.game = game;

            InitializeComponent();
        }

        void playerDifficultyChanged(object sender, EventArgs e)
        {

            RadioButton radioButton = sender as RadioButton;

            if (radioButton != null)
            {

                if (radioButton.Checked)
                {

                    _vsType = radioButton;
                    if(_vsType == pvc)
                    {
                        playerColorGroup.Visible = true;
                        difficultyDropDown.Visible = true;
                        DifficultyLabel.Visible = true;
                    }
                    else if (_vsType == pvp)
                    {
                        playerColorGroup.Visible = false;
                        difficultyDropDown.Visible = false;
                        DifficultyLabel.Visible = false;
                    }
                    else //vstype == cvc
                    {
                        difficultyDropDown.Visible = true;
                        DifficultyLabel.Visible = true;
                        playerColorGroup.Visible = false;
                    }
                }
            }

        }

        private void OkClick(object sender, EventArgs e)
        {            
            if(pvc.Checked)
            {
                game.vs_type = CheckerGame.VS_TYPE.PLAYER_VS_CPU;
                game.difficulty = CheckerGame.difficultyList[difficultyDropDown.SelectedIndex];
                game.PlayerColor = playerColorRed.Checked ? 0 : 1;
            }
            else if (cvc.Checked)
            {
                game.vs_type = CheckerGame.VS_TYPE.CPU_VS_CPU;
                game.difficulty = CheckerGame.difficultyList[difficultyDropDown.SelectedIndex];
            }
            else // (checkedRadioButton == pvp)
            {
                game.vs_type = CheckerGame.VS_TYPE.PLAYER_VS_PLAYER;
            }
            game.Renew();
            game.currentGameStatus = CheckerGame.GameStatus.IN_PROGRESS;
            Close();
        }

        private void mode_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void close_button_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetupForm_Load(object sender, EventArgs e)
        {

        }

        private void difficultyDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void difficultyDropDown_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            
        }

        private void playerColorChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                if (radioButton.Checked)
                {
                    _playerColor = radioButton;                   
                }
            }
        }

    }
}
