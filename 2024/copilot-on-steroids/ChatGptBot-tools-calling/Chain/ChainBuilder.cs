using ChatGptBot.Ioc;

namespace ChatGptBot.Chain
{
    public interface ILangChainBuilder
    {
        ChainBuilder Add(ILangChainBrick brick);
        ILangChainBrick BuildChain();
    }
    public class ChainBuilder : ILangChainBuilder, ISingletonScope
    {
        private ILangChainBrick?  _current ;
        private ILangChainBrick? _first;

        public ChainBuilder Add(ILangChainBrick brickBase)
        {
            if (_current == null)
            {
                _first = brickBase;
            }
            else
            {
                _current.SetNext(brickBase);
            }

            _current = brickBase;    
            return this;
        }

        public ILangChainBrick BuildChain()
        {
            if (_first == null)
            {
                throw new Exception("no LangChainItemBase provided");
            }
            return _first;
        }
    }
}
