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

namespace Wizards_tale
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class LevelWindow : Window
    {
        readonly int levelInd;
        List<int> choosedSpells = new List<int>();
        bool isWin = false;
        int AIturn;
        int turnInd = 0;
        Random rand = new Random(DateTime.Now.Millisecond);

        int opponentArmor;
        int playerArmor;
        double opponentAttackMultiplier;
        double playerAttackMultiplier;
        double playerDodgeChance;
        bool isDebuffActive;
        bool isUltimateUsed;

        public LevelWindow() : this(1)
        {
        }

        public LevelWindow(int levelInd)
        {
            InitializeComponent();
            this.levelInd = levelInd;

            // get database info
            opponentNameLabel.Content = "Страж кровавой зари";
            opponentHP.Maximum = 1600;
            opponentHP.Value = 1600;
            playerHP.Maximum = 1000;
            playerHP.Value = 1000;
            opponentArmor = 0;
            playerArmor = 0;
            isUltimateUsed = false;
            opponentAttackMultiplier = 1;
            playerAttackMultiplier = 1;
            playerDodgeChance = 0;
            opponentImage.Source = new BitmapImage(new Uri("assets/opponentSprites/1levelenemy.png", UriKind.Relative));
            infoLabel.Text = (string)TryFindResource("startInfoLevel1");
            TakeSpellsFromDeck();

            TextControlPanel.Visibility = Visibility.Visible;
            TextControlPanel.textLabel.Text = (string)TryFindResource("level1entry");
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            MapWindow mapWindow = new MapWindow(levelInd + 1);
            mapWindow.Top = Top;
            mapWindow.Left = Left;
            mapWindow.Show();
            Close();
        }

        private void nextTurn_Click(object sender, RoutedEventArgs e)
        {
            if (turnInd == 0)
            {
                CastPlayerSpell(choosedSpells);
                choosedSpells.Clear();
                choosedSkillsLabel.Text = "";
                skillsGrid.IsEnabled = false;
                nextTurn.Content = "Далее";

                if (isDebuffActive)
                {
                    isDebuffActive = false;
                }
                else
                {
                    button_5.IsEnabled = true;
                    button_6.IsEnabled = true;
                }
            }
            else if (turnInd == 1)
            {
                CastAISpell();
                skillsGrid.IsEnabled = false;
                nextTurn.Content = "Далее";
            }
            else if (turnInd == 2)
            {
                TakeSpellsFromDeck();
                ChooseAITurn();
                skillsGrid.IsEnabled = true;
                nextTurn.Content = "Разыграть заклинание";
            }
            else
            {
                int newLvlInd = levelInd;
                if (isWin) newLvlInd++;

                MapWindow mapWindow = new MapWindow(newLvlInd);
                mapWindow.Top = Top;
                mapWindow.Left = Left;
                mapWindow.Show();
                Close();
            }

            turnInd++;
            if (turnInd > 2) turnInd = 0;

            if (opponentHP.Value <= 0 || playerHP.Value <= 0)
            {
                if (opponentHP.Value <= 0)
                {
                    infoLabel.Text = "Отлично, дракон-страж пал!";
                    isWin = true;
                }
                else
                {
                    infoLabel.Text = "Ох, кажется противник оказался сильнее. Хорошо, что у вас есть заклинание воскрешения :)";
                }

                button_1.IsEnabled = false;
                button_2.IsEnabled = false;
                button_3.IsEnabled = false;
                button_4.IsEnabled = false;
                button_5.IsEnabled = false;
                button_6.IsEnabled = false;

                nextTurn.Content = "Назад к карте";
                turnInd = 3;
            }
        }

        private void SkillButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is int buttonContent)
            {
                
                if (choosedSpells.Contains(buttonContent))
                {
                    choosedSpells.Remove(buttonContent);
                }
                else
                {
                    choosedSpells.Add(buttonContent);
                }
            }

            if(choosedSpells.Count == 0)
            {
                choosedSkillsLabel.Text = "";
                return;
            }

            choosedSkillsLabel.Text = "";
            foreach (var spell in choosedSpells)
            {
                choosedSkillsLabel.Text += (string)TryFindResource("spell" + spell.ToString()) + ", ";
            }
            choosedSkillsLabel.Text = (choosedSkillsLabel.Text as string).Substring(0, (choosedSkillsLabel.Text as string).Length - 2);
        }


        // GameLogic

        private void TakeSpellsFromDeck()
        {
            List<int> tmpList = new List<int>();
            for (int i = 1; i < 19; i++)
            {
                tmpList.Add(i);
            }

            var newSpells = tmpList.OrderBy(x => rand.Next()).Take(6).ToList();
            int ind = 0;
            
            foreach (var item in skillsGrid.Children)
            {
                if (item is Button button)
                {
                    button.Content = newSpells[ind];
                    ImageBrush imgBrush;
                    switch (newSpells[ind])
                    {
                        case 1:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/1.firestrike.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 2:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/2.firestorm.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 3:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/3.atkBuff.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 4:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/4.thunderStrike.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 5:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/5.thunderStrong.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 6:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/6.thunderStaff.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 7:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/7.waterStrike.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 8:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/8.iceStrike.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 9:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/9.freeze.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 10:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/10.circleArrow.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 11:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/11.poisonIvy.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 12:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/12.death.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 13:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/13.chains.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 14:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/14.eye.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 15:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/15.stealth.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 16:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/16.heal.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 17:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/17.combineBlock.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                        case 18:
                            imgBrush = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/spells/18.random.png")));
                            imgBrush.Stretch = Stretch.Uniform;
                            button.Background = imgBrush;
                            break;
                    }

                    ind++;
                }
            }
        }

        private void CastPlayerSpell(List<int> spellsArr)
        {
            infoLabel.Text = "";

            //a lot of spells
            if (spellsArr.Count > 4)
            {
                infoLabel.Text = "Заклинания смешались в кучу, но что-то произошло...";

                switch (rand.Next(3))
                {
                    case 0:
                        opponentAttackMultiplier *= 0.7;
                        break;
                    case 1:
                        DamageOpponent(100);
                        break;
                    case 2:
                        AddPlayerArmor(50);
                        break;
                }
                return;
            }

            // discrete spells
            if (spellsArr.Contains(17))
            {
                foreach (var spell in spellsArr)
                {
                    CastSpell(spell);
                }

                return;
            }

            // massive fire storm
            if (spellsArr.Contains(1) && spellsArr.Contains(2) && spellsArr.Contains(3))
            {
                infoLabel.Text = "Призванный вами огненный шторм пожег вашего врага и нанес ему огромные увечья";
                DamageOpponent(900);
                AddOpponentArmor(-100);
                return;
            }

            // paralysis
            if (spellsArr.Contains(4) && spellsArr.Contains(5) && spellsArr.Contains(6))
            {
                infoLabel.Text = "Вы парализовали противника. Иногда, он будет пропускать ходы";
                return;
            }

            // water thing
            if (spellsArr.Contains(7) && spellsArr.Contains(8) && spellsArr.Contains(9))
            {
                infoLabel.Text = "Кажется, вы сильно ослабили противника";
                opponentAttackMultiplier *= 0.5;
                AddOpponentArmor(-300);
                return;
            }

            // random death
            if (spellsArr.Contains(10) && spellsArr.Contains(11) && spellsArr.Contains(12))
            {
                infoLabel.Text = "Ваше заклинание слишком сильно! Оно вырвалось из-под вашего контроля. Оно не остановится, пока кто-то не погибнет";
                if (rand.Next(3) == 0)
                {
                    playerHP.Value = 0;
                }
                else
                {
                    opponentHP.Value = 0;
                    isWin = true;
                }

                button_1.IsEnabled = false;
                button_2.IsEnabled = false;
                button_3.IsEnabled = false;
                button_4.IsEnabled = false;
                button_5.IsEnabled = false;
                button_6.IsEnabled = false;

                nextTurn.Content = "Назад к карте";
                turnInd = 3;
                return;
            }

            // TODO: turn skip
            /*
            if (spellsArr.Contains(13) && spellsArr.Contains(14) && spellsArr.Contains(15))
            {

                return;
            }
            */

            // escape
            if (spellsArr.Contains(9) && spellsArr.Contains(13) && spellsArr.Contains(15))
            {
                infoLabel.Text = "Вы смогли задержать дракона и убежать!";
            
                button_1.IsEnabled = false;
                button_2.IsEnabled = false;
                button_3.IsEnabled = false;
                button_4.IsEnabled = false;
                button_5.IsEnabled = false;
                button_6.IsEnabled = false;

                nextTurn.Content = "Назад к карте";
                turnInd = 3;
                return;
            }

            // take enemy spell
            if (spellsArr.Contains(14) && spellsArr.Contains(10) && spellsArr.Contains(9))
            {
                infoLabel.Text = "Ваш набор заклинаний пополнился! Вы сможете использовать новое заклинание в следующем бою";
                return;
            }

            // armour
            if (spellsArr.Contains(9) && spellsArr.Contains(16))
            {
                AddPlayerArmor(150);
                infoLabel.Text += "Вы чувствуете, как ваша броня наполняется мощью\n";
            }

            // replay enemy spell
            if (spellsArr.Contains(14) && spellsArr.Contains(10))
            {
                switch (AIturn)
                {
                    case 0:
                        AddPlayerArmor(150);
                        infoLabel.Text += "Вы чувствуете, как ваша броня наполняется мощью\n";
                        break;
                    case 1:
                        DamageOpponent(250);
                        infoLabel.Text += "Отлично! Ваш удар настиг врага\n";
                        break;
                    case 2:
                        opponentAttackMultiplier *= 0.7;
                        infoLabel.Text += "Оппонент ослаблен\n";
                        break;
                    case 3:
                        playerHP.Maximum += 400;
                        playerHP.Value += 400;
                        playerAttackMultiplier *= 1.2;
                        infoLabel.Text += "Вы чувствуете как силы наполняют вас\n";
                        break;
                }
            }

            // massive thunder damage
            if ((spellsArr.Contains(4) || spellsArr.Contains(5) || spellsArr.Contains(6)) && spellsArr.Contains(7))
            {
                DamageOpponent(800);
                infoLabel.Text += "Отлично! Комбинация оказалась удачной\n";
            }

            // low damage
            if ((spellsArr.Contains(1) || spellsArr.Contains(2) || spellsArr.Contains(3)) && spellsArr.Contains(7))
            {
                DamageOpponent(150);
                infoLabel.Text += "Заклинание имело плохую эффективность\n";
            }

            spellsArr = spellsArr.OrderBy(x => rand.Next()).ToList();
            CastSpell(spellsArr[0]);

            if (spellsArr.Count > 1 && rand.Next(3) != 0)
            {
                CastSpell(spellsArr[1]);
            }
        }

        
        private void CastSpell(int spellInd)
        {
            if (spellInd == 17) return;

            if (spellInd == 18) spellInd = rand.Next(16) + 1;

            switch (spellInd)
            {
                case 1:
                    DamageOpponent(400);
                    infoLabel.Text += "Большой шар огня направился в сторону врага и нанес ему серьезный урон\n";
                    break;
                case 2:
                    DamageOpponent(300);
                    AddOpponentArmor(-50);
                    infoLabel.Text += "Кажется, противник получил серьезные ожоги\n";
                    break;
                case 3:
                    playerAttackMultiplier *= 1.3;
                    infoLabel.Text += "Вы чувствуете себя сильнее\n";
                    break;
                case 4:
                    DamageOpponent(350);
                    infoLabel.Text += "Молния пронзила тело врага, поражаю каждую клетку\n";
                    break;
                case 5:
                    DamageOpponent(200);
                    AddOpponentArmor(-100);
                    infoLabel.Text += "Противник не скоро оправится от такого удара\n";
                    break;
                case 6:
                    playerDodgeChance += 0.1;
                    if (playerDodgeChance > 0.5) playerDodgeChance = 0.5;
                    infoLabel.Text += "Противник теперь хуже контролирует свои действия\n";
                    break;
                case 7:
                    opponentAttackMultiplier *= 0.9;
                    infoLabel.Text += "Большая струя воды помешала планам оппонента\n";
                    break;
                case 8:
                    opponentAttackMultiplier *= 0.95;
                    DamageOpponent(50);
                    infoLabel.Text += "Лед нанес оппоненту несколько порезов\n";
                    break;
                case 9:
                    // TODO: smth here
                    infoLabel.Text += "Вы наложили страную печать\n";
                    break;
                case 10:
                    // TODO: last spell
                    infoLabel.Text += "Вы повторили предыдущее заклинание\n";
                    break;
                case 11:
                    AddOpponentArmor(-200);
                    infoLabel.Text += "Ядовитые лозы оплетают оппонента, отравляя его\n";
                    break;
                case 12:
                    if (rand.Next(2) == 0)
                    {
                        DamageOpponent(600);
                    }
                    else
                    {
                        DamagePlayer(550);
                    }
                    infoLabel.Text += "Вы использовали трудноконтролируемое заклинание, надеясь, что оно нанесет урон врагу, а не вам\n";
                    break;
                case 13:
                    //TODO: speed
                    infoLabel.Text += "Цепи сильно замедлили врага\n";
                    break;
                case 14:
                    isUltimateUsed = true;
                    infoLabel.Text += "Противник теперь не может использовать некоторые свои заклятия\n";
                    break;
                case 15:
                    playerDodgeChance += 0.2;
                    if (playerDodgeChance > 0.4) playerDodgeChance = 0.4;
                    infoLabel.Text += "Вы скрываетесь в тенях, противнику сложнее в вас попасть\n";
                    break;
                case 16:
                    playerHP.Value += 400;
                    infoLabel.Text += "Ваши силы восполнились\n";
                    break;
            }
        }

        private void AddOpponentArmor(int armor)
        {
            opponentArmor += armor;
            if (opponentArmor < 0) opponentArmor = 0;
            opponentArmorLabel.Content = opponentArmor;
        }

        private void AddPlayerArmor(int armor)
        {
            playerArmor += armor;
            if (playerArmor < 0) playerArmor = 0;
            playerArmorLabel.Content = playerArmor;
        }

        private void DamagePlayer(int damage)
        {
            if (rand.NextDouble() < playerDodgeChance)
            {
                infoLabel.Text += "Вы уклонились\n";
                return;
            }

            damage = (int)(damage * opponentAttackMultiplier);
            damage -= playerArmor;
            if (damage < 0) damage = 0;

            playerHP.Value -= damage;
            if (playerHP.Value < 0) playerHP.Value = 0;
        }

        private void DamageOpponent(int damage)
        {
            damage = (int)(damage * playerAttackMultiplier);
            damage -= opponentArmor;
            if (damage < 0) damage = 0;

            opponentHP.Value -= damage;
            if (opponentHP.Value < 0) opponentHP.Value = 0;
        }

        private void ChooseAITurn()
        {
            if (isUltimateUsed)
                AIturn = rand.Next(3);
            else
                AIturn = rand.Next(4);

            switch (AIturn)
            {
                case 0:
                    infoLabel.Text = (string)TryFindResource("defendInfo" + (rand.Next(2) + 1).ToString());
                    break;
                case 1:
                    infoLabel.Text = (string)TryFindResource("attackInfo" + (rand.Next(2) + 1).ToString());
                    break;
                case 2:
                    infoLabel.Text = (string)TryFindResource("debuffInfo" + (rand.Next(2) + 1).ToString());
                    break;
                case 3:
                    infoLabel.Text = (string)TryFindResource("ultimateInfo" + (rand.Next(2) + 1).ToString());
                    break;
            } 
        }

        private void CastAISpell()
        {
            switch (AIturn)
            {
                case 0:
                    AddOpponentArmor(150);
                    infoLabel.Text = "Дракон выглядит мощнее";
                    break;
                case 1:
                    DamagePlayer(600);
                    infoLabel.Text = "Новая рана располагается прямо на вашей груди";
                    break;
                case 2:
                    button_5.IsEnabled = false;
                    button_6.IsEnabled = false;
                    isDebuffActive = true;
                    infoLabel.Text = "Кажется, придется обойтись без нескольких заклинаний";
                    break;
                case 3:
                    opponentImage.Source = new BitmapImage(new Uri("assets/opponentSprites/1levelenemyUltimate.png", UriKind.Relative));
                    opponentImage.Stretch = Stretch.UniformToFill;
                    opponentHP.Maximum += 500;
                    opponentHP.Value += 500;
                    opponentAttackMultiplier = 1.5;
                    isUltimateUsed = true;
                    infoLabel.Text = "\"Теперь начинается настоящий бой!\"";
                    break;
            }
        }
    }
}
