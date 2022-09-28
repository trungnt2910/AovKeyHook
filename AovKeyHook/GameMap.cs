using Gma.System.MouseKeyHook.WinForms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AovKeyHook
{
    static class GameMap
    {
        public static readonly Size ScreenSize = new Size(1570, 923);

        public static readonly Point Skill1Upgrade = new Point(1086, 720);

        public static readonly Point Skill2Upgrade = new Point(1189, 547);

        public static readonly Point Skill3Upgrade = new Point(1345, 459);

        public static readonly Point Skill1 = new Point(1170, 805);

        public static readonly Point Skill2 = new Point(1260, 635);

        public static readonly Point Skill3 = new Point(1425, 550);

        public static readonly Point SkillSpecial = new Point(1380, 360);

        public static readonly Point Ability1 = new Point(795, 820);

        public static readonly Point Ability2 = new Point(915, 820);

        public static readonly Point Ability3 = new Point(1035, 820);

        public static readonly Point Attack = new Point(1427, 778);
        
        public static readonly Point Tower = new Point(1500, 653);

        public static readonly Point Minion = new Point(1300, 840);

        public static readonly Point Item1 = new Point(176, 372);

        public static readonly Point Item2 = new Point(175, 464);

        public static readonly Point Status = new Point(1470, 80);

        public static readonly Point JoystickCenter = new Point(200, 760);

        static GameMap()
        {
            
        }

        public static Point? HandleKeyUp(Keys key)
        {
            return key switch
            {
                Keys.Q => Skill1,
                Keys.W => Skill2,
                Keys.E => Skill3,
                Keys.R => SkillSpecial,
                Keys.D1 => Skill1Upgrade,
                Keys.D2 => Skill2Upgrade,
                Keys.D3 => Skill3Upgrade,
                Keys.Space => Attack,
                Keys.A => Minion,
                Keys.S => Tower,
                Keys.B => Ability1,
                Keys.D => Ability2,
                Keys.F => Ability3,
                Keys.Z => Item1,
                Keys.X => Item2,
                Keys.Tab => Status,
                _ => null
            };
        }

        public static Point? HandleKeyDown(Keys key)
        {
            return key switch
            {
                Keys.Tab => Status,
                _ => null,
            };
        }
    }
}
