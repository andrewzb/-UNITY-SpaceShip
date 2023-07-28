namespace SpaceShip.Models
{
    public struct LobbyData
    {
        public LobbyData(string name)
        {
            Name = name;
            MaxPlayers = 2;
        }
        
        public string Name { get; private set; }
        public int MaxPlayers { get; private set; }
    }
}