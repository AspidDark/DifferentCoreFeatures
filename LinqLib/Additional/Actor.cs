using System;
using System.Collections.Generic;
using System.Text;

namespace LinqLib.Additional
{
    public class Actor
    {
        public int birthYear;
        public string firstName;
        public string lastName;

        public static Actor[] GetActors()
        {
            Actor[] actors = new Actor[] {
        new Actor { birthYear = 1964, firstName = "Keanu", lastName = "Reeves" },
        new Actor { birthYear = 1968, firstName = "Owen", lastName = "Wilson" },
        new Actor { birthYear = 1960, firstName = "James", lastName = "Spader" },
        new Actor { birthYear = 1964, firstName = "Sandra", lastName = "Bullock" },
      };

            return (actors);
        }
    }
}
