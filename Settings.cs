using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GammaJul.LgLcd;

namespace MusicBeePlugin
{
  public partial class Settings : Form
  {
    private string settingsPath_;
    public string backgroundImage_;
    public bool alwaysOnTop_;
    public bool useDefaultBackground_;
    public int volumeChanger_;
    public MusicBeePlugin.Plugin.screenEnum startupScreen_;
    public List<string> screenList_ = new List<string>();
    private Plugin plugin_;
    private bool monochromeDevice_;

    public Settings(string settingspath, Plugin plugin)
    {
      InitializeComponent();

      settingsPath_ = settingspath + "LogitechLCD\\";
      Directory.CreateDirectory(settingsPath_);

      plugin_ = plugin;

      disabledScreensList.Items.AddRange(typeof(MusicBeePlugin.Plugin.screenEnum).GetEnumNames());

      defaultScreencomboBox.DropDownStyle = ComboBoxStyle.DropDownList;

      AlwaysOnTopRadioButton.Checked = true;
      BackgroundDefaultButton.Checked = true;

      openSettings();
    }

    public void setDevice(LcdDevice device)
    {
      if (device != null)
      {
        if (device.DeviceType == LcdDeviceType.Monochrome)
        {
          monochromeDevice_ = true;

          logitechLabel.Text = "Monochrome Logitech device found. (G15, G510, ...).";

          backgroundLabel.Visible = false;
          BackgroundDefaultButton.Visible = false;
          BackgroundCustomButton.Visible = false;
          browseButton.Visible = false;
        }
        else if (device.DeviceType == LcdDeviceType.Qvga)
        {
          monochromeDevice_ = false;

          logitechLabel.Text = "Color Logitech device found. (G19).";

          backgroundLabel.Visible = true;
          BackgroundDefaultButton.Visible = true;
          BackgroundCustomButton.Visible = true;
          browseButton.Visible = true;
        }
      }
    }

    public void openSettings()
    {
      bool alwaysOnTop = true;
      int volume = 10;

      try
      {
        string[] lines = File.ReadAllLines(settingsPath_ + "LogitechLCDSettings.ini");

        if (lines.Length > 0)
        {
          string alwaysOnTopString = lines[0].Replace("alwaysOnTop: ", "");
          string volumeString = lines[1].Replace("volumeChanger: ", "");
          string defaultBackground = lines[2].Replace("DefaultBackground: ", "");
          string backgroundString = lines[3].Replace("Background: ", "");
          string pageCounterString = lines[4].Replace("PageCounter: ", "");
          string pagesString = lines[5].Replace("Pages: ", "");
          string startupScreen = lines[6].Replace("StartScreen: ", "");

          volume = Convert.ToInt32(volumeString);
          alwaysOnTop = Convert.ToBoolean(alwaysOnTopString);
          useDefaultBackground_ = Convert.ToBoolean(defaultBackground);
          backgroundImage_ = backgroundString;
          int counter = Convert.ToInt32(pageCounterString);

          while (pagesString.Length != 0)
          {
            string temp = pagesString.Substring(0, pagesString.IndexOf(";"));
            pagesString = pagesString.Remove(0, pagesString.IndexOf(";")+1);

            temp = temp.Replace(";", "");

            screenList_.Add(temp);
          }

          startupScreen_ = (MusicBeePlugin.Plugin.screenEnum)Enum.Parse(typeof(MusicBeePlugin.Plugin.screenEnum), startupScreen);

        }
        if (alwaysOnTop)
        {
          this.AlwaysOnTopRadioButton.Checked = true;
        }
        else
        {
          this.AlwaysOnTopRadioButton.Checked = false;
        }

        if (useDefaultBackground_)
        {
          BackgroundDefaultButton.Checked = true;
        }
        else
        {
          BackgroundDefaultButton.Checked = false;
        }

        this.volumeChangerSpinBox.Value = volume;
      }
      catch (Exception)
      {

      }
    }

    private void radioButton1_CheckedChanged(object sender, EventArgs e)
    {
      alwaysOnTop_ = true;
    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.Hide();
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
      string errorMessage = "";

      if (enabledScreensList.Items.Count == 0)
      {
        errorMessage += "You need to select at least one screen\n";
      }

      if (!(defaultScreencomboBox.SelectedIndex > -1))
      {
        errorMessage += "You need to select an start screen\n";
      }

      if (monochromeDevice_)
      {
        int pagecounter = enabledScreensList.Items.Count;

        string[] problemArray = typeof(MusicBeePlugin.Plugin.ScreenUseAllButtons).GetEnumNames();

        for (int i = 0; i < pagecounter; i++)
        {
          for (int j = 0; j < problemArray.Length; j++)
          {
            bool test = (i != 0 || i != pagecounter - 1);

            if (enabledScreensList.Items[i].ToString() == problemArray[j] && !(i == 0 || i == pagecounter - 1))
            {
              errorMessage += "Screen" + problemArray[j] + " must be in first position or last position\n";
            }
          }
        }

      }

      if (errorMessage.Length == 0)
      {
        screenList_.Clear();

        string screenSave = "";

        for (int i = 0; i < enabledScreensList.Items.Count; i++)
        {
          screenSave += enabledScreensList.Items[i].ToString();
          screenSave += ";";
          screenList_.Add(enabledScreensList.Items[i].ToString());
        }

        string[] lines = { "alwaysOnTop: " + alwaysOnTop_, "volumeChanger: " + volumeChanger_, "DefaultBackground: " + BackgroundCustomButton.Checked,
                         "Background: " + backgroundImage_, "PageCounter: " + enabledScreensList.Items.Count, "Pages: " + screenSave, "StartScreen: " + defaultScreencomboBox.Items[defaultScreencomboBox.SelectedIndex].ToString()};
        File.WriteAllLines(settingsPath_ + "LogitechLCDSettings.ini", lines);

        openSettings();

        plugin_.settingsChanged();

        this.Hide();
      }
      else
      {
        MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void radioButton2_CheckedChanged(object sender, EventArgs e)
    {
      alwaysOnTop_ = false;

    }

    private void label3_Click(object sender, EventArgs e)
    {

    }

    private void label3_Click_1(object sender, EventArgs e)
    {

    }

    private void BackgroundDefaultButton_CheckedChanged(object sender, EventArgs e)
    {
      browseButton.Enabled = false;

      useDefaultBackground_ = true;

    }

    private void BackgroundCustomButton_CheckedChanged(object sender, EventArgs e)
    {
      //BackgroundDefaultButton.Checked = false;
      //BackgroundCustomButton.Checked = true;
      browseButton.Enabled = true;

      useDefaultBackground_ = false;
    }

    private void browseButton_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Image Files (*.jpg)|*.jpg";
      DialogResult result = dialog.ShowDialog(this);

      if (result == System.Windows.Forms.DialogResult.OK)
      {
        File.Copy(dialog.FileName, settingsPath_ + "background.jpg");

        backgroundImage_ = "background.jpg";
      }
    }

    private void volumeChangerSpinBox_ValueChanged(object sender, EventArgs e)
    {
      volumeChanger_ = (int)volumeChangerSpinBox.Value;
    }

    private void defaultScreencomboBox_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void enableButton_Click(object sender, EventArgs e)
    {
      if (disabledScreensList.SelectedItem != null)
      {
        defaultScreencomboBox.Items.Add(disabledScreensList.SelectedItem);
        enabledScreensList.Items.Add(disabledScreensList.SelectedItem);
        disabledScreensList.Items.Remove(disabledScreensList.SelectedItem);
      }
    }

    private void disableButton_Click(object sender, EventArgs e)
    {
      if (enabledScreensList.SelectedItem != null)
      {
        defaultScreencomboBox.Items.Remove(enabledScreensList.SelectedItem);
        disabledScreensList.Items.Add(enabledScreensList.SelectedItem);
        enabledScreensList.Items.Remove(enabledScreensList.SelectedItem);
      }
    }

    private void upButton_Click(object sender, EventArgs e)
    {
      if (enabledScreensList.SelectedItem != null)
      {
        int currentIndex = enabledScreensList.SelectedIndex;
        var item = enabledScreensList.Items[currentIndex];
        if (currentIndex > 0)
        {
          enabledScreensList.Items.RemoveAt(currentIndex);
          enabledScreensList.Items.Insert(currentIndex - 1, item);
        }
      }
    }

    private void downButton_Click(object sender, EventArgs e)
    {
      if (enabledScreensList.SelectedItem != null)
      {
        int currentIndex = enabledScreensList.SelectedIndex;
        var item = enabledScreensList.Items[currentIndex];
        if (currentIndex < enabledScreensList.Items.Count - 1)
        {
          enabledScreensList.Items.RemoveAt(currentIndex);
          enabledScreensList.Items.Insert(currentIndex + 1, item);
        }
      }
    }
  }
}
