namespace Assets.Classes
{
    public class Message
    {
        private Enum_Action command;
        private int dataIndex = 0;
        public byte[] data;

        //constructor
        public Message(Enum_Action action)
        {
            command = action;
            data = new byte[63];
        }

        //read constructor
        public Message(byte[] buffer, int receivedSize)
        {
            data = new byte[receivedSize - 1];
            command = (Enum_Action)buffer[0];

            for (int i = 1; i < receivedSize; i++)
            {
                data[i - 1] = buffer[i];
            }
        }

        //add data to array
        public void AddData(byte value)
        {
            data[dataIndex] = value;
            dataIndex++;
        }

        public byte[] GetBuffer()
        {
            //create correct sized buffer
            byte[] message = new byte[dataIndex + 1];

            if (message.Length <= 1)
            {
                return null;
            }

            message[0] = (byte)command;
            for (var i = 0; i < dataIndex; i++)
            {
                message[i + 1] = data[i];
            }

            return message;
        }

        public int GetSize()
        {
            return dataIndex + 1;
        }

        public Enum_Action GetCommand()
        {
            return command;
        }
    }
}
