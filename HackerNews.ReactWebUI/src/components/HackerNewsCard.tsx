import Moment from "moment";
import {
  Card,
  CardBody,
  Stack,
  StackDivider,
  HStack,
  Heading,
  Text,
  Link,
} from "@chakra-ui/react";
import { HackerNewsStory } from "../hooks/useHackerNewsStories";

interface Props {
  newsStory: HackerNewsStory;
}

const HackerNewsCard = ({ newsStory }: Props) => {
  return (
    <Card>
      <CardBody>
        <Stack divider={<StackDivider />} spacing="2">
          <Heading fontSize="xl">{newsStory.title}</Heading>
          <Text pt="2" fontSize="sm">
            Posted By:{" "}
            <Link color="teal.500" href={newsStory.uri} isExternal>
              {newsStory.postedBy}
            </Link>
          </Text>
          <HStack justifyContent="space-between" marginBottom={3}>
            <Text>Score: {newsStory.score}</Text>
            <Text>Comments: {newsStory.commentCount}</Text>
            <Text>Posted: {Moment(newsStory.time).format("LLL")}</Text>
          </HStack>
        </Stack>
      </CardBody>
    </Card>
  );
};

export default HackerNewsCard;
