// @ts-expect-error 'we are using the resolved settings from public/env-config.js'
const env = window?._env_;

class ConstUtils {
    static IsNotEmpty = (val: string | undefined): boolean => {
        return val !== undefined && val !== "" && val.length > 0;
    }

    static GetStringValue = (val: string | undefined, defaultFn: () => string): string => {
        return this.IsNotEmpty(val) ? val as string : defaultFn();
    }
}

class Consts {
    static readonly HackerNewsApiBaseUrl: string = ConstUtils.GetStringValue(env?.HACKERNEWS_API_BASE_URL, () => 'http://localhost:8090');
}

export class UrlResolver {
    static Resolve = (baseUrl: string, relativeUrl: string): string => {
        return new URL(relativeUrl, baseUrl).href;
    };
}

export default Consts;
