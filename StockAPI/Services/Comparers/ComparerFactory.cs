namespace StockApi.Services.Comparers
{
    public class ComparerFactory
    {
        public ComparerFactory()
        {
        }

        public FieldComparer CreateComparer(string field)
        {
            switch (field)
            {
                case "本益比": return new PeRatioComparer();
                case "股價淨值比": return new PbRatioComparer();
                case "殖利率(%)": return new DividendYieldComparer();
                default : return null;
            }
        }

    }
    
}
