using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Leo.WindowModels;

namespace Leo.PageModels
{
    public partial class About
    {
        public About() { InitializeComponent(); }

        private void licLinkMouseEnter(object sender, MouseEventArgs e)
        { LicenseLink.Visibility = Visibility.Visible; }
        private void licLinkClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo 
                { FileName = "https://raw.githubusercontent.com/MenshovAnton/VoiceAssistantLeo/master/LICENSE", UseShellExecute = true });
        }
        private void licLinkMouseLeave(object sender, MouseEventArgs e)
        { LicenseLink.Visibility = Visibility.Hidden; }

        private void repoLinkMouseEnter(object sender, MouseEventArgs e)
        { RepoLink.Visibility = Visibility.Visible; }
        private void repoLinkClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo 
                { FileName = "https://github.com/MenshovAnton/VoiceAssistantLeo", UseShellExecute = true });
        }
        private void repoLinkMouseLeave(object sender, MouseEventArgs e)
        { RepoLink.Visibility = Visibility.Hidden; }

        private void issuesLinkMouseEnter(object sender, MouseEventArgs e)
        { IssuesLink.Visibility = Visibility.Visible; }
        private void issuesLinkClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo
                { FileName = "https://github.com/MenshovAnton/VoiceAssistantLeo/issues", UseShellExecute = true });
        }
        private void issuesLinkMouseLeave(object sender, MouseEventArgs e)
        { IssuesLink.Visibility = Visibility.Hidden; }
        
        private void siteLinkMouseEnter(object sender, MouseEventArgs e)
        { SiteLink.Visibility = Visibility.Visible; }
        private void siteLinkClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo 
                { FileName = "https://voiceassistantleo.tilda.ws/ru/home", UseShellExecute = true });
        }
        private void siteLinkMouseLeave(object sender, MouseEventArgs e)
        { SiteLink.Visibility = Visibility.Hidden; }
        
        private void skillsLinkMouseEnter(object sender, MouseEventArgs e)
        { SkillsLink.Visibility = Visibility.Visible; }
        private void skillsLinkClick(object sender, MouseButtonEventArgs e)
        { MainWindow.getSkillsPage(); }
        private void skillsLinkMouseLeave(object sender, MouseEventArgs e)
        { SkillsLink.Visibility = Visibility.Hidden; }
    }
}
