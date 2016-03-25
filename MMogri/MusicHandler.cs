using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MMogri.Sound
{
    //Taken directly from the API at: https://msdn.microsoft.com/en-us/library/4fe3hdb1%28v=vs.110%29.aspx


    class MusicHandler
    {
        public static void Play(Note[] tune, bool looping = false)
        {
            Thread thread = new Thread(() =>
          {
              while (looping)
              {
                  foreach (Note n in tune)
                  {
                      if (n.NoteTone == Tone.REST)
                          Thread.Sleep((int)n.NoteDuration);
                      else
                          Console.Beep((int)n.NoteTone, (int)n.NoteDuration);
                  }
              }
          });
            thread.Start();
        }

        public enum Tone
        {
            REST = 0,
            GbelowC = 196,
            A = 220,
            Asharp = 233,
            B = 247,
            C = 262,
            Csharp = 277,
            D = 294,
            Dsharp = 311,
            E = 330,
            F = 349,
            Fsharp = 370,
            G = 392,
            Gsharp = 415,
        }

        public enum Duration
        {
            WHOLE = 1600,
            HALF = WHOLE / 2,
            QUARTER = HALF / 2,
            EIGHTH = QUARTER / 2,
            SIXTEENTH = EIGHTH / 2,
        }

        public struct Note
        {
            Tone toneVal;
            Duration durVal;

            public Note(Tone frequency, Duration time)
            {
                toneVal = frequency;
                durVal = time;
            }

            public Tone NoteTone { get { return toneVal; } }
            public Duration NoteDuration { get { return durVal; } }
        }
    }
}
