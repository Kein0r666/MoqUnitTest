using Microsoft.Extensions.DependencyInjection;
using MoqUnitTest.Moq.UnitTest.Injector;
using System;
using System.Collections.Generic;
using Users.BaseIdentity;
using Users.IIdentityService;
using WebSocketControllers.Core.ExtensionsCore;
using WebSocketControllers.Core.Factory;
using WebSocketControllers.Core.Models;

namespace Tirscript.WebSocketDependencyInjector
{
    public class WebSocketDependencyInjector<TUser, TUserId> : DependencyInjector
        where TUser : class, IUser<TUserId>
        where TUserId : IEquatable<TUserId>
    {
        public TUser DefultUser { get; set; }

        public WebSocketDependencyInjector(string jsonPath = "")
            :base(jsonPath)
        {
        }

        public void Prepare<TIdentityService>(TUser user = null)
            where TIdentityService : class, IUsersIdentityService<TUser, TUserId>
            
        {
            Services.AddWebSocket(Configuration);
            Services.AddTransient<IUsersIdentityService<TUser, TUserId>, TIdentityService>();

            DefultUser = user;
        }
        public WsUser<TUser, TUserId> FakeUser()
        {
            return new WsUser<TUser, TUserId>(FakeSocket(), ServiceProvider)
            {
                User = DefultUser
            };
        }
        public WsUser<TUser, TUserId> FakeUser(TUser user)
        {
            return new WsUser<TUser, TUserId>(FakeSocket(), ServiceProvider)
            {
                User = user
            };
        }
        public T AuthorizeService<T>()
        {
            var service = GetService<T>();

            (service as WsController<TUser, TUserId>).User = FakeUser();
            (service as WsController<TUser, TUserId>).Socket = FakeSocket();
            return service;
        }
        public T AuthorizeUser<T>(T service, TUser user, Guid tokenId)
        {
            (service as WsController<TUser, TUserId>).User = FakeUser(user);
            (service as WsController<TUser, TUserId>).Socket = FakeSocket(tokenId);
            return service;
        }
        public T AuthorizeUser<T>(TUser user, Guid tokenId)
        {
            var service = GetService<T>();

            (service as WsController<TUser, TUserId>).User = FakeUser(user);
            (service as WsController<TUser, TUserId>).Socket = FakeSocket(tokenId);
            return service;
        }
        public T AuthorizeUser<T>((TUser user, Guid tokenId) user)
        {
            var service = GetService<T>();

            (service as WsController<TUser, TUserId>).User = FakeUser(user.user);
            (service as WsController<TUser, TUserId>).Socket = FakeSocket(user.tokenId);
            return service;
        }
        public void AuthorizeUserService<T>(TUser user, Guid tokenId)
        {
            var service = GetService<T>();

            (service as WsController<TUser, TUserId>).User = FakeUser(user);
            (service as WsController<TUser, TUserId>).Socket = FakeSocket(tokenId);
        }
        public void AuthorizeUserService<T>((TUser user, Guid tokenId) user)
        {
            var service = GetService<T>();

            (service as WsController<TUser, TUserId>).User = FakeUser(user.user);
            (service as WsController<TUser, TUserId>).Socket = FakeSocket(user.tokenId);
        }
        public WebSocketConnection FakeSocket()
        {
            return new WebSocketConnection(null, null, null, Guid.NewGuid(), ServiceProvider);
        }
        public WebSocketConnection FakeSocket(Guid tokenId)
        {
            return new WebSocketConnection(null, null, null, tokenId, ServiceProvider);
        }

        public IUsersIdentityService<TUser, TUserId> GetUsersIdentityService()
        {
            return this.GetService<IUsersIdentityService<TUser, TUserId>>();
        }

        public virtual (TUser User, TUserId SessionId) CreateUser()
        {
            return (DefultUser, DefultUser.Id);
        }
    }
}
