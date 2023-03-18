using System;

namespace lab4_4_4
{
    public class Action1
    {
        public void Execute()
        {
            Console.WriteLine("Action 1 executed.");
        }
    }

    public class Action2
    {
        public void Execute()
        {
            Console.WriteLine("Action 2 executed.");
        }
    }

    public class Action3
    {
        public void Execute()
        {
            Console.WriteLine("Action 3 executed.");
        }
    }


    public class Workflow
    {

        public event Action<Action1> OnAction1;
        public event Action<Action2> OnAction2;
        public event Action<Action3> OnAction3;


        public void Start()
        {

            OnAction1?.Invoke(new Action1());
        }


        private void HandleAction1(Action1 action)
        {
            Console.WriteLine("Handling Action 1...");

            OnAction2?.Invoke(new Action2());
        }

        private void HandleAction2(Action2 action)
        {
            Console.WriteLine("Handling Action 2...");

            OnAction3?.Invoke(new Action3());
        }

        private void HandleAction3(Action3 action)
        {
            Console.WriteLine("Handling Action 3...");

        }

        public Workflow()
        {
            OnAction1 += HandleAction1;
            OnAction2 += HandleAction2;
            OnAction3 += HandleAction3;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var workflow = new Workflow();
            workflow.Start();
            Console.ReadLine();
        }
    }
}


