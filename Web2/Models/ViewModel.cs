using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Web2.Models
{
    public class ViewModel
    {
        public decimal StartAmount { get; set; }
        public int PlayersNumber { get; set; }
    }

    public class InputParams
    {

        public decimal InitAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public int CountPeople { get; set; }

    }

    public class PaymentService
    {
        private InputParams inputParams;
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private CancellationToken token;
        private object lockObj = new object();


        public PaymentService(InputParams inputParams)
        {
            this.inputParams = inputParams;
            token = cancelTokenSource.Token;
        }

        public async Task<InputParams> Start()
        {
            this.inputParams.BalanceAmount = this.inputParams.InitAmount;
            List<Task> players = new List<Task>();
            for (var i = 0; i < this.inputParams.CountPeople; i++)
            {
                Task task = new Task(TakeAmount, token);
                players.Add(task);
            }
            players.ForEach(x => x.Start());

            Task.WaitAny(players.ToArray());
            return this.inputParams;
        }

        private void TakeAmount()
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                lock (lockObj)
                {
                    if (CanTakeMore())
                    {
                        //Delay for data base trunsaction
                        Task.Delay(100);
                        this.inputParams.BalanceAmount = this.inputParams.BalanceAmount - 0.1M;
                    }
                    else
                    {
                        cancelTokenSource.Cancel();
                        return;
                    }
                }
            }
        }

        private bool CanTakeMore()
        {
            if (this.inputParams.BalanceAmount > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
