import { Heading } from "@chakra-ui/react";
import { HackerNewsQuery } from "../App";

interface Props {
  newsQuery: HackerNewsQuery;
}

const HackerNewsHeading = ({ newsQuery }: Props) => {
  const heading = `${newsQuery?.name || ""} News Stories`;

  return (
    <Heading as="h1" marginY={5} fontSize="5xl">
      {heading}
    </Heading>
  );
};

export default HackerNewsHeading;
