namespace Channels;

//2 publishers  1 reader
public class ChannelData
{
    public string StringData { get; set; }

    public string ChannelName { get; set; }
}

//1 publiser  2 readers same message foreach reader KafkaLike

public class ChannelData2
{
    public string StringData { get; set; }

    public string ChannelName { get; set; }
}


//1 publiser  2 readers differnt messages processing
public class ChannelData3
{
    public string StringData { get; set; }

    public string ChannelName { get; set; }
}

