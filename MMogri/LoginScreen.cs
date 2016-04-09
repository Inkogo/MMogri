﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Renderer;
using MMogri.Input;

namespace MMogri
{
    class LoginScreen : ContentFrame
    {
        ClientMain client;

        public LoginScreen(GameWindow w, InputHandler i, ClientMain c) : base(w, i)
        {
            client = c;
        }

        override public void Start()
        {
            //DrawFrame(2, 2, window.sizeX - 4, window.sizeY - 4);
            //window.SetEditMode(true);
        }

        public void LoginAccount()
        {
            Console.WriteLine("[J] Join Account");
            Console.WriteLine("[C] Create account");
            Console.WriteLine("[R] Forgot Password");

            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.J)
            {
                Console.WriteLine("Email:");
                string email = Console.ReadLine();
                Console.WriteLine("Password:");
                string pword = Console.ReadLine();

                client.RequestLoginAccount(email, pword.ToGuid());
            }
            else if (key.Key == ConsoleKey.C)
            {
                Console.WriteLine("Email:");
                string email = Console.ReadLine();
                Console.WriteLine("Password:");
                string pword = Console.ReadLine();

                client.RequestCreateAccount(email, pword.ToGuid());
            }
            else if (key.Key == ConsoleKey.R)
            {
                Console.WriteLine("Email:");
                string email = Console.ReadLine();

                client.RequestResetPassword(email);
            }
            else
                Console.WriteLine("Unknown command!");

            window.Clear();
        }

        public void LoginPlayer(string[] p, Guid account, Guid session)
        {
            Console.WriteLine("Logged in Succesfully!");

            if (p.Length == 0)
                Console.WriteLine("No players foound!");

            int n = 0;
            foreach (string s in p)
            {
                Console.WriteLine("[" + (n + 1) + "] " + s);
                n++;
            }
            Console.WriteLine("");
            Console.WriteLine("[C] Create new player");
            Console.WriteLine("[N] Change password");

            ConsoleKeyInfo key = Console.ReadKey(true);
            int i = 0;
            if (int.TryParse(key.KeyChar.ToString(), out i))
            {
                client.RequestSpawn(p[i - 1], account, session);
            }
            else if (key.Key == ConsoleKey.C)
            {
                Console.WriteLine("Name:");
                string name = Console.ReadLine();

                client.RequestCreatePlayer(name, account, session);
            }
            else if (key.Key == ConsoleKey.N)
            {
                Console.WriteLine("Old Password:");
                string oldP = Console.ReadLine();
                Console.WriteLine("New Password:");
                string newP = Console.ReadLine();

                client.RequestChangePassword(oldP.ToGuid(), newP.ToGuid(), account, session);
            }
        }
    }
}