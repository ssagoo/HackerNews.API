import { HackerNewsQuery } from "../App";
import useData from "./useData";

export interface HackerNewsStory {
    id: number;
    title: string;
    uri: string;
    postedBy: string;
    time: Date;
    score: number;
    commentCount: number;
}

const useHackerNewsStories = (hackerNewsQuery: HackerNewsQuery) => useData<HackerNewsStory>("/beststories", {
    params: {
        maxStories: hackerNewsQuery.maxStories
    }
},
    [hackerNewsQuery]);

export default useHackerNewsStories;
