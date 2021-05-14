namespace Demo.Domain.Entities
{
    public class User : Entity
    {
        #region Properties

        public uint Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        #endregion
    }
}