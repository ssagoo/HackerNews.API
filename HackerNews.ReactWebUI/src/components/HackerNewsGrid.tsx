import { SimpleGrid, Text } from "@chakra-ui/react";
import useHackerNewsStories from "../hooks/useHackerNewsStories";
import HackerNewsCard from "./HackerNewsCard";
import HackerNewsCardSkeleton from "./HackerNewsCardSkeleton";
import HackerNewsCardContainer from "./HackerNewsCardContainer";
import { HackerNewsQuery } from "../App";

interface Props {
  hackerNewsQuery: HackerNewsQuery;
}

const HackerNewsGrid = ({ hackerNewsQuery }: Props) => {
  const { data, error, isLoading } = useHackerNewsStories(hackerNewsQuery);
  const skeletons = [1, 2, 3, 4, 5, 6];

  if (error) return <Text>{error}</Text>;

  return (
    <SimpleGrid
      columns={{ sm: 1, md: 2, lg: 3, xl: 4 }}
      padding="10px"
      spacing={6}
    >
      {isLoading &&
        skeletons.map((skeleton) => (
          <HackerNewsCardContainer key={skeleton}>
            <HackerNewsCardSkeleton />
          </HackerNewsCardContainer>
        ))}
      {data.map((newsStory) => (
        <HackerNewsCardContainer key={newsStory.id}>
          <HackerNewsCard newsStory={newsStory} />
        </HackerNewsCardContainer>
      ))}
    </SimpleGrid>
  );
};

export default HackerNewsGrid;
