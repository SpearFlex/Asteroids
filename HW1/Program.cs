using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGame
{

    class Program
      {
          static void Main(string[] args)
          {
              Form form = new Form
              {
                  Width = Screen.PrimaryScreen.Bounds.Width,
                  Height = Screen.PrimaryScreen.Bounds.Height
              };
              Game.Init(form);
              form.Show();
              Game.Load();
              Game.Draw();
              Application.Run(form);
          }
      }
  }