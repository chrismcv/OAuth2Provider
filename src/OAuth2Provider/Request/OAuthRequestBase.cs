﻿using System;
using System.Collections.Generic;
using System.Web;
using OAuth2Provider.Validation;
using log4net;

namespace OAuth2Provider.Request
{
    public abstract class OAuthRequestBase : IOAuthRequest
    {
        private readonly IRequest _request;
        private IList<string> _requiredTokens = new List<string>();
        protected readonly ILog Logger = LogManager.GetLogger(typeof(TokenRequest));

        protected OAuthRequestBase(IRequest request, IOAuthServiceLocator serviceLocator)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");

            _request = request;
            ServiceLocator = serviceLocator;

            Validate();
        }

        private void Validate()
        {
            var validator = GetRequestValidator();

            var result = validator.ValidateRequest(this);
            if (!result.Success)
                throw new OAuthException(result.ErrorCode, result.ErrorDescription, result.ErrorUri);
        }

        protected abstract IRequestValidator GetRequestValidator();

        protected IOAuthServiceLocator ServiceLocator { get; set; }

        public string GrantType
        {
            get { return _request.Properties.SafeGetValue(OAuthTokens.GrantType); }
        }
        public string Username
        {
            get { return _request.Properties.SafeGetValue(OAuthTokens.Username); }
        }
        public string Password
        {
            get { return _request.Properties.SafeGetValue(OAuthTokens.Password); }
        }
        public string Scope
        {
            get { return _request.Properties.SafeGetValue(OAuthTokens.Scope); }
        }
        public string AccessToken
        {
            get
            {
                var accessToken = _request.Properties.SafeGetValue(OAuthTokens.AccessToken);
                if (!string.IsNullOrWhiteSpace(accessToken))
                    return accessToken;

                accessToken = _request.Properties.SafeGetValue(OAuthTokens.OAuthToken);
                if (!string.IsNullOrWhiteSpace(accessToken))
                    return accessToken;

                var authHeader = _request.Headers.SafeGetValue(HeaderType.Authorization) + "";
                if(authHeader.Contains("OAuth") || authHeader.Contains("Bearer"))
                    accessToken = authHeader.Replace("OAuth ", "").Replace("Bearer ", "").Trim();

                return accessToken;
            }
        }
        public string TokenType
        {
            get { return _request.Properties.SafeGetValue(OAuthTokens.TokenType); }
        }
        public string ExpiresIn
        {
            get { return _request.Properties.SafeGetValue(OAuthTokens.ExpiresIn); }
        }
        public string RefreshToken
        {
            get { return _request.Properties.SafeGetValue(OAuthTokens.RefreshToken); }
        }
        public string ResponseType
        {
            get { return _request.Properties.SafeGetValue(OAuthTokens.ResponseType); }
        }
        public string RedirectUri
        {
            get { return _request.Properties.SafeGetValue(OAuthTokens.RedirectUri); }
        }
        public string State
        {
            get { return _request.Properties.SafeGetValue(OAuthTokens.State); }
        }
        public string ClientId
        {
            get
            {
                var clientID = _request.Properties.SafeGetValue(OAuthTokens.ClientId);
                if (!string.IsNullOrWhiteSpace(clientID))
                    return clientID;

                return BasicAuthenticationScheme.Username;
            }
        }
        public string ClientSecret
        {
            get
            {

                var clientID = _request.Properties.SafeGetValue(OAuthTokens.ClientSecret);
                if (!string.IsNullOrWhiteSpace(clientID))
                    return clientID;

                return BasicAuthenticationScheme.Password;
            }
        }
        public string ContentType
        {
            get { return _request.ContentType; }
        }
        public string AuthorizationCode
        {
            get { return _request.Properties.SafeGetValue(OAuthTokens.Code); }
        }
        public string Method
        {
            get { return _request.HttpMethod; }
        }
        public bool IsSecure
        {
            get { return _request.IsSecure; }
        }

        private HttpBasicAuthenticationScheme _basicAuthenticationScheme;
        private HttpBasicAuthenticationScheme BasicAuthenticationScheme
        {
            get
            {
                if (_basicAuthenticationScheme == null)
                    _basicAuthenticationScheme = _request.DecodeBasicAuthentication();

                return _basicAuthenticationScheme;
            }
        }
    }

}