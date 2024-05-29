namespace RisePayTest.Models.AuxiliaryModels
{
   public class CardResponse<T>
    {
        public int LengthTotalRegisters { get; set; }
        public int LengthFiltredResults { get; set; }
        public List<T> Result { get; set; }
    }

    public class CardParameters
    {
        public int ResultsQtd { get; set; }
        public string SearchValue { get; set; }
        public List<int> Ignore { get; set; }
       


    }
}
