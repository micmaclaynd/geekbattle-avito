using ApiGateway.Configurations;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Shared.Configuration.Extensions;

namespace ApiGateway.Services {
    public interface IRouterService {
        RouteConfiguration? GetRouteByPath(string path);
        Uri GetNestedUri(RouteConfiguration route, string path);
        Uri AddQueryString(Uri uri, string queryString);
        string GetDefaultContentType();
        string GetDefaultTransferEncoding();
    }

    public class RouterService : IRouterService {
        private readonly IConfiguration _configuration;
        private readonly RouterConfiguration _router;

        public RouterService(IConfiguration configuration) {
            _configuration = configuration;
            _router = _configuration.GetSectionOrThrow<RouterConfiguration>("ApiGateway:Router");
        }

        public RouteConfiguration? GetRouteByPath(string path) {
            foreach (var route in _router.Routes.Where(route => route.IsNestedUrls == false)) {
                if (route.Path == path) {
                    return route;
                }
            }

            foreach (var route in _router.Routes.Where(route => route.IsNestedUrls == true)) {
                var baseUri = new UriBuilder() {
                    Path = route.Path,
                }.Uri;
                var routeUri = new UriBuilder() {
                    Path = path
                }.Uri;

                if (IsUriNested(routeUri, baseUri)) {
                    return route;
                }
            }

            return null;
        }

        public Uri GetNestedUri(RouteConfiguration route, string path) {
            var baseUri = new Uri($"{route.ProxyUri.GetLeftPart(UriPartial.Authority)}{route.Path}");
            var routeUri = new Uri($"{route.ProxyUri.GetLeftPart(UriPartial.Authority)}{path}");
            var relativeUri = baseUri.MakeRelativeUri(routeUri);
            return new Uri(route.ProxyUri, relativeUri);
        }

        public Uri AddQueryString(Uri uri, string queryString) {
            var builder = new UriBuilder(uri) {
                Query = queryString
            };
            return builder.Uri;
        }

        public bool IsUriNested(Uri uri1, Uri uri2) {
            return uri1.AbsoluteUri.StartsWith(uri2.AbsoluteUri, StringComparison.OrdinalIgnoreCase);
        }

        public string GetDefaultContentType() {
            return _router.DefaultContentType;
        }

        public string GetDefaultTransferEncoding() {
            return _router.DefaultTransferEncoding;
        }
    }
}
