namespace Capgemini.PowerApps.SpecFlowBindings.Transformations.Tokenisation
{
    using System.Collections.Generic;
    using Capgemini.PowerApps.SpecFlowBindings.Transformations.Tokenisation.TokenHandlers;

    /// <summary>
    /// Factory to return all the token handlers.
    /// </summary>
    public static class TokenFactory
    {
        /// <summary>
        /// Return all handlers for Tokenisation.
        /// </summary>
        /// <returns>All the registered Handlers.</returns>
        public static List<BaseTokenHandler> GetHandlers()
        {
            return new List<BaseTokenHandler>
            {
                new StringTokenReplacer(),
                new IntegerTokenReplacer(),
                new DateTimeTokenReplacer(),
            };
        }
    }
}
