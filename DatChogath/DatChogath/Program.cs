using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Color = System.Drawing.Color;


namespace DatChogath
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;

        }
        private static AIHeroClient User = Player.Instance; // our player.

        // Declaring spells.

        // Q
        public static Spell.Skillshot Q;
        //W
        public static Spell.Skillshot W;
        //E
        public static Spell.Active E;
        //R      
        public static Spell.Targeted R;
        // Declaring Menus.

        private static Menu ChogathMenu, ComboMenu;

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (User.ChampionName != "Chogath")
            {
                return;
            }

            // Q spell Values
            Q = new Spell.Skillshot(spellSlot: SpellSlot.Q, spellRange: 950, skillShotType: SkillShotType.Circular, castDelay: 650, spellSpeed: null, spellWidth: 250);

            // W Spell Values
            W = new Spell.Skillshot(spellSlot: SpellSlot.W, spellRange: 650, skillShotType: SkillShotType.Cone, castDelay: 25, spellSpeed: int.MaxValue, spellWidth: (int)(30 * 0.5));

            // E Spell Values
            E = new Spell.Active(spellSlot: SpellSlot.E);

            // R Spell Values
            R = new Spell.Targeted(spellSlot: SpellSlot.R, spellRange: 250);

            // Creating the MainMenu
            ChogathMenu = MainMenu.AddMenu("DatChogath", "DatChogath");

            // Sub ComboMenu
            ComboMenu = ChogathMenu.AddSubMenu("Combo Settings");

            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("E", new CheckBox("Use E"));
            ComboMenu.Add("Z", new CheckBox("Use Z"));

            // wtf
            Game.OnTick += Game_OnTick;
        }


            private static void Game_OnTick(EventArgs args)
        {

        }







        }
    }


