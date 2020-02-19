﻿using System;
using System.Collections.Generic;
using Barebones.Networking;

namespace Barebones.MasterServer{
    public class MsfAuthClient : MsfBaseClient{
        public delegate void LoginCallback(AccountInfoPacket accountInfo, string error);

        private bool _isLogginIn;

        public bool IsLoggedIn { get; private set; }

        public AccountInfoPacket AccountInfo;

        public event Action LoggedIn;
        public event Action Registered;
        public event Action LoggedOut;

        public MsfAuthClient(IClientSocket connection) : base(connection){}

        #region Register User (Not currently used)
        /// <summary>
        /// Sends a registration request to server
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void Register(Dictionary<string, string> data, SuccessCallback callback){
            Register(data, callback, Connection);
        }

        /// <summary>
        /// Sends a registration request to given connection
        /// </summary>
        public void Register(Dictionary<string, string> data, SuccessCallback callback, IClientSocket connection){
            if (_isLogginIn){
                callback.Invoke(false, "Log in is already in progress");
                return;
            }

            if (IsLoggedIn){
                callback.Invoke(false, "Already logged in");
                return;
            }

            if (!connection.IsConnected){
                callback.Invoke(false, "Not connected to server");
                return;
            }

            // We first need to get an aes key 
            // so that we can encrypt our login data
            Msf.Security.GetAesKey(aesKey =>{
                if (aesKey == null){
                    callback.Invoke(false, "Failed to register due to security issues");
                    return;
                }

                var encryptedData = Msf.Security.EncryptAES(data.ToBytes(), aesKey);

                connection.SendMessage((short)MsfOpCodes.RegisterAccount, encryptedData, (status, response) =>{

                    if (status != ResponseStatus.Success) {
                        callback.Invoke(false, response.AsString("Unknown error"));
                        return;
                    }

                    callback.Invoke(true, null);

                    if (Registered != null)
                        Registered.Invoke();
                });
            }, connection);
        }
        #endregion

        #region LogOut
        /// <summary>
        ///     Initiates a log out. In the process, disconnects and connects
        ///     back to the server to ensure no state data is left on the server.
        /// </summary>
        public void LogOut(){
            LogOut(Connection);
        }

        /// <summary>
        ///     Initiates a log out. In the process, disconnects and connects
        ///     back to the server to ensure no state data is left on the server.
        /// </summary>
		/// 
        public void LogOut(IClientSocket connection){
            if (!IsLoggedIn)
                return;

            IsLoggedIn = false;
            AccountInfo = null;

            if ((connection != null) && connection.IsConnected)
                connection.Reconnect();

            if (LoggedOut != null)
                LoggedOut.Invoke();
        }
        #endregion


		public void LoginAsAlbotUser(LoginCallback callback, string username, string password, string clientVersion){
			LogIn(new Dictionary<string, string>(){
					{AlbotDictKeys.username, username },
					{AlbotDictKeys.password, password},
                    {AlbotDictKeys.clientVersion, clientVersion }
				}, callback, Connection);
		}

        /// <summary>
        /// Sends a generic login request
        /// </summary>
        public void LogIn(Dictionary<string, string> data, LoginCallback callback, IClientSocket connection){
            if (!connection.IsConnected){
                callback.Invoke(null, "Not connected to server");
                return;
            }

            _isLogginIn = true;

            // We first need to get an aes key 
            // so that we can encrypt our login data
            Msf.Security.GetAesKey(aesKey =>{
                if (aesKey == null) {
                    _isLogginIn = false;
                    callback.Invoke(null, "Failed to log in due to security issues");
                    return;
                }
                
                var encryptedData = Msf.Security.EncryptAES(data.ToBytes(), aesKey);

				connection.SendMessage((short) CustomMasterServerMSG.login, encryptedData, (status, response) =>{
                    _isLogginIn = false;

                    if (status != ResponseStatus.Success){
                        callback.Invoke(null, response.AsString("Unknown error"));
                        return;
                    }

                    IsLoggedIn = true;

                    AccountInfo = response.Deserialize(new AccountInfoPacket());
					callback.Invoke(AccountInfo, null);
                });
            }, connection);
        }

        /// <summary>
        /// Sends an e-mail confirmation code to the server
        /// </summary>
        /// <param name="code"></param>
        /// <param name="callback"></param>
        public void ConfirmEmail(string code, SuccessCallback callback){
            ConfirmEmail(code, callback, Connection);
        }

        /// <summary>
        /// Sends an e-mail confirmation code to the server
        /// </summary>
        public void ConfirmEmail(string code, SuccessCallback callback, IClientSocket connection){
            if (!connection.IsConnected){
                callback.Invoke(false, "Not connected to server");
                return;
            }

            if (!IsLoggedIn){
                callback.Invoke(false, "You're not logged in");
                return;
            }

            connection.SendMessage((short)MsfOpCodes.ConfirmEmail, code, (status, response) =>{
                if (status != ResponseStatus.Success){
                    callback.Invoke(false, response.AsString("Unknown error"));
                    return;
                }

                callback.Invoke(true, null);
            });
        }

        /// <summary>
        /// Sends a request to server, to ask for an e-mail confirmation code
        /// </summary>
        /// <param name="callback"></param>
        public void RequestEmailConfirmationCode(SuccessCallback callback){
            RequestEmailConfirmationCode(callback, Connection);
        }

        /// <summary>
        /// Sends a request to server, to ask for an e-mail confirmation code
        /// </summary>
        public void RequestEmailConfirmationCode(SuccessCallback callback, IClientSocket connection){
            if (!connection.IsConnected){
                callback.Invoke(false, "Not connected to server");
                return;
            }

            if (!IsLoggedIn){
                callback.Invoke(false, "You're not logged in");
                return;
            }

            connection.SendMessage((short)MsfOpCodes.RequestEmailConfirmCode, (status, response) =>{
                if (status != ResponseStatus.Success){
                    callback.Invoke(false, response.AsString("Unknown error"));
                    return;
                }

                callback.Invoke(true, null);
            });
        }

        /// <summary>
        /// Sends a request to server, to ask for a password reset
        /// </summary>
        public void RequestPasswordReset(string email, SuccessCallback callback){
            RequestPasswordReset(email, callback, Connection);
        }

        /// <summary>
        /// Sends a request to server, to ask for a password reset
        /// </summary>
        public void RequestPasswordReset(string email, SuccessCallback callback, IClientSocket connection){
            if (!connection.IsConnected){
                callback.Invoke(false, "Not connected to server");
                return;
            }

            connection.SendMessage((short)MsfOpCodes.PasswordResetCodeRequest, email, (status, response) =>{
                if (status != ResponseStatus.Success){
                    callback.Invoke(false, response.AsString("Unknown error"));
                    return;
                }

                callback.Invoke(true, null);
            });
        }

        /// <summary>
        /// Sends a new password to server
        /// </summary>
        public void ChangePassword(PasswordChangeData data, SuccessCallback callback){
            ChangePassword(data, callback, Connection);
        }

        /// <summary>
        /// Sends a new password to server
        /// </summary>
        public void ChangePassword(PasswordChangeData data, SuccessCallback callback, IClientSocket connection){
            if (!connection.IsConnected){
                callback.Invoke(false, "Not connected to server");
                return;
            }

            var dictionary = new Dictionary<string, string>(){
                {"email", data.Email },
                {"code", data.Code },
                {"password", data.NewPassword }
            };

            connection.SendMessage((short)MsfOpCodes.PasswordChange, dictionary.ToBytes(), (status, response) =>{
                if (status != ResponseStatus.Success){
                    callback.Invoke(false, response.AsString("Unknown error"));
                    return;
                }

                callback.Invoke(true, null);
            });
        }
    }
}