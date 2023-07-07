using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyGame
{
    static class Game
    {

        private static Timer _timer = new Timer() { Interval = 55 };
        public static Random Rnd = new Random();

        public static int Score;

        const int MAX_HEIGHT = 1000, MAX_WIDTH = 1000;

        private static Ship _ship = new Ship(new Point(10, 400), new Point(5, 5), new Size(50, 50));

        private static Fuel[] _fuel;
        private static Bullet _bullet;
        private static Asteroid[] _asteroids;
        public static BaseObject[] _objs;
        
        public static void Load()
        {
            _objs = new BaseObject[30];
            _asteroids = new Asteroid[8];
            _fuel = new Fuel[3];

            for (var i = 0; i < _objs.Length; i++)
            {
                int r = Rnd.Next(5, 50);
                _objs[i] = new Star(new Point(1000, Rnd.Next(0, Game.Height)), new Point(-r, r), new Size(3, 3));
            }

            for (var i = 0; i < _asteroids.Length; i++)
            {
                int r = Rnd.Next(5, 50);
                _asteroids[i] = new Asteroid(new Point(1000, Rnd.Next(0, Game.Height)), new Point(-r+5, r), new Size(r*2, r*2));
            }

            for (var i = 0; i < _fuel.Length; i++)
            {
                int r = Rnd.Next(3, 12);
                _fuel[i] = new Fuel (new Point(1000, Rnd.Next(0, Game.Height)), new Point(-r , r), new Size(r*2, r*2));
            }

        }

        public static void Finish()
        {
            _timer.Stop();
            Buffer.Graphics.DrawString("The End. Score: " + Score, new Font(FontFamily.GenericSansSerif, 60, FontStyle.Underline), Brushes.White, 200, 100);
            Buffer.Render();
        }




        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;

        public static int Width { get; set; }
        public static int Height { get; set; }



        static Game()
        {
        }

        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) _bullet = new Bullet(new Point(_ship.Rect.X + 10, _ship.Rect.Y + 25), new Point(4, 0), new Size(4, 1));
            if (e.KeyCode == Keys.Up) _ship.Up();
            if (e.KeyCode == Keys.Down) _ship.Down();
        }


        public static void Init(Form form)
        {
            Graphics g;
            _context = BufferedGraphicsManager.Current;
            g = form.CreateGraphics();

            form.KeyDown += Form_KeyDown;
            Ship.MessageDie += Finish;

            Width = form.ClientSize.Width;
            Height = form.ClientSize.Height;

            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));

            _timer.Start();
            _timer.Tick += Timer_Tick;
        }
        private static void Timer_Tick(object sender, EventArgs e)
        {
            Draw();
            Update();
        }

        public static void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);
            foreach (BaseObject obj in _objs)
                obj.Draw();
            foreach (Asteroid a in _asteroids)
            {
                a?.Draw();
            }
            _bullet?.Draw();
            _ship?.Draw();

            foreach (Fuel f in _fuel)
            {
                f?.Draw();
            }

            if (_ship != null)
                Buffer.Graphics.DrawString("Energy: " + _ship.Energy, SystemFonts.DefaultFont, Brushes.White, 0, 0);

            Buffer.Graphics.DrawString("Score: " + Score, SystemFonts.DefaultFont, Brushes.Azure, 100, 0);

            Buffer.Render();
        }

        public static void Update()
        {
            foreach (BaseObject obj in _objs) obj.Update();
            _bullet?.Update();

            for(var i = 0; i < _fuel.Length; i++)
            {
                if (_fuel[i] == null)
                    continue;
                _fuel[i].Update();
                if (_ship != null && _ship.Collision(_fuel[i]))
                {

                    System.Media.SystemSounds.Beep.Play();
                    _ship?.EnergyHigh(Rnd.Next(10, 30));
                    _fuel[i] = null;
                    continue;
                }
            }

            for (var i = 0; i < _asteroids.Length; i++)
            {
                if (_asteroids[i] == null)
                continue;

                _asteroids[i].Update();
                if (_bullet != null && _bullet.Collision(_asteroids[i]))
                {
                    System.Media.SystemSounds.Hand.Play();
                    Score += 25;
                    _asteroids[i] = null;
                    _bullet = null;
                    continue;
                }
                if (!_ship.Collision(_asteroids[i])) continue;
                _ship?.EnergyLow(Rnd.Next(1, 10));
                System.Media.SystemSounds.Asterisk.Play();
                if (_ship.Energy <= 0) _ship?.Die();
            }





            for (var i = 0; i < _asteroids.Length; i++)
            {
                if (_asteroids[i] == null)
                    _asteroids[i] = new Asteroid(new Point(1000, Rnd.Next(0, Game.Height)), new Point(-13, 25), new Size(40, 40));
            }

                for (var i = 0; i < _fuel.Length; i++)
                {
                    if (_fuel[i] == null)
                        _fuel[i] = new Fuel(new Point(1000, Rnd.Next(0, Game.Height)), new Point(-13, 25), new Size(10, 10));

                }
        }
    }
}


