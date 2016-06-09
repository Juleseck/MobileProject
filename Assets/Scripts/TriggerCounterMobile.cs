namespace Assets.Scripts
{
    public static class TriggerCounterMobile
    {
        public static int TotalDevicesConnected { get; set; }

        public static int Klappen { get; private set; }
        public static int Springen { get; private set; }
        public static int Wave { get; private set; }

        public static void IncreaseKlappen()
        {
            //Klappen = (Klappen == 0) ? 1 : Klappen + 1;
            Klappen++;
        }

        public static void IncreaseSpringen()
        {
            //Springen = (Springen == 0) ? 1 : Springen + 1;
            Springen++;
        }

        public static void IncreaseWave()
        {
            //Wave = (Wave == 0) ? 1 : Wave + 1;
            Wave++;
        }

        public static void DecreaseKlappen()
        {
            if (Klappen == 0) return;
            Klappen--;
        }

        public static void DecreaseSpringen()
        {
            if (Springen == 0) return;
            Springen--;
        }

        public static void DecreaseWave()
        {
            if (Wave == 0) return;
            Wave--;
        }
    }
}

