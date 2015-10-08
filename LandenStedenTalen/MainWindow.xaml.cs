using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LandenStedenTalen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            VulLanden();           
        }
        Land land = new Land();
        private void LbLanden_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LbLanden.SelectedItem != null)
            {
                land = (Land)LbLanden.SelectedItem;
                VulSteden();
                VulTalen();
            }
        }
        private void VulLanden()
        {
            using (var entities = new LandenStedenTalenEntities())
            {
                LbLanden.ItemsSource = (from land in entities.Landen
                                        orderby land.Naam
                                        select land).ToList();
            }
        }
        private void VulSteden()
        {
            using (var entities = new LandenStedenTalenEntities())
            {
                lbSteden.ItemsSource = (from stad in entities.Steden
                                        orderby stad.Naam
                                        where stad.LandCode == land.LandCode
                                        select stad).ToList();
            }
        }
        private void VulTalen()
        {
            using (var entities = new LandenStedenTalenEntities())
            {
                var talenList = (from taal in entities.Talen
                                 select taal).ToList();

                List<Taal> taalPerLand = new List<Taal>();

                foreach (var taal in talenList)
                {
                    foreach (var landTaal in taal.Landen)
                    {
                        if (landTaal.Naam == land.Naam)
                            taalPerLand.Add(taal);
                    }
                }

                lbTalen.ItemsSource = taalPerLand.OrderBy(t => t.Naam);
            }
        }
        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbIngaveStad.Text != "")
                {
                    if (LbLanden.SelectedItem != null)
                    {
                       
                        var stad = new Stad { Naam = (ToUpperFirst(tbIngaveStad.Text)).ToString(), LandCode = land.LandCode};
                        var selectedLand = (Land)LbLanden.SelectedItem;
                        using (var entities = new LandenStedenTalenEntities())
                        {
                            var stadLookUp = entities.Steden.Where(st => st.Naam == stad.Naam && st.LandCode == selectedLand.LandCode).FirstOrDefault();
                            if (stadLookUp == null)
                            {
                                entities.Steden.Add(stad);
                                entities.SaveChanges();
                            }
                            else
                            {
                                tbIngaveStad.Clear();
                                throw new ArgumentException("De stad "+ stad.Naam +" is al aanwezig in de stedenlijst van " + selectedLand.Naam + " !!!");
                            }
                        }
                        VulSteden();
                    }
                    else
                        throw new ArgumentException("Er is geen land geselecteerd!!!");
                }
                else
                    throw new ArgumentException("De textbox is leeg!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }
        }
        private string ToUpperFirst(string s)
        {
            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }
    }
}
