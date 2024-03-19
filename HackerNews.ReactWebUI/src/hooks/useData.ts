import { AxiosRequestConfig } from "axios";
import apiClient, { CanceledError } from "../services/api-client";
import { useEffect, useState } from "react";


interface FetchResponse<T> {
    count: number;
    results: T[];
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const useData = <T>(endPoint: string, requestConfig?: AxiosRequestConfig, deps?: any[]) => {
    const [data, setData] = useState<T[]>([]);
    const [error, setError] = useState("");
    const [isLoading, setLoading] = useState(false);

    useEffect(() => {
        const controller = new AbortController();

        setLoading(true);
        apiClient
            .get<FetchResponse<T>>(endPoint, { signal: controller.signal, ...requestConfig })
            .then((res) => {
                setData(res.data.results);
                setLoading(false);
            })
            .catch((err) => {
                setLoading(false);
                if (err instanceof CanceledError) return;
                setError(err.message);
            });

        return () => controller.abort();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, deps ? [...deps] : []);

    return { data, error, isLoading };

}

export default useData;