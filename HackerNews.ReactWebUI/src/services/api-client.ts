import axios, { AxiosError, CanceledError } from "axios";
import Consts from "./GlobalConsts";

export default axios.create({
    params: {
    },
    baseURL: Consts.HackerNewsApiBaseUrl,
    //headers: {}
});

export { CanceledError, AxiosError };
