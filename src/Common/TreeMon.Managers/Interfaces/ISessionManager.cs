// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
namespace TreeMon.Managers.Interfaces
{
    public interface ISessionManager
    {
        #region Interface member methods.
        /// <summary>
        ///  Function to generate unique token with expiry against the provided userUUID.
        ///  Also add a record in database for generated token.
        /// </summary>
        /// <param name="userUUID"></param>
        /// <returns></returns>
        string GenerateToken(int userUUID);

        /// <summary>
        /// Function to validate token againt expiry and existance in database.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        bool ValidateToken(string tokenId);

        /// <summary>
        /// Method to kill the provided token id.
        /// </summary>
        /// <param name="tokenId"></param>
        bool Kill(string tokenId);

        /// <summary>
        /// Delete tokens for the specific deleted user
        /// </summary>
        /// <param name="userUUID"></param>
        /// <returns></returns>
        bool DeleteByUserId(int userUUID);
        #endregion
    }
}
