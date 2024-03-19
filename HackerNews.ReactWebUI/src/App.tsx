import { Box, Grid, GridItem } from "@chakra-ui/react";
import { useState } from "react";
import NavBar from "./components/NavBar";
import HackerNewsGrid from "./components/HackerNewsGrid";
import HackerNewsHeading from "./components/HackerNewsHeading";

export interface HackerNewsQuery {
  name: string;
  maxStories?: number;
}

function App() {
  const [hackerNewsQuery, setHackerNewsQuery] = useState<HackerNewsQuery>({
    maxStories: 1,
  } as HackerNewsQuery);

  const getMaxStories = (searchText: string): number | undefined => {
    let maxStories: number | undefined = searchText ? parseInt(searchText) : 5;
    if (isNaN(maxStories)) {
      maxStories = 1;
    }
    return maxStories;
  };

  return (
    <Grid
      templateAreas={{
        base: `"nav" "main"`,
      }}
      templateColumns={{
        base: "1fr", // 1 fraction, take up all available space
      }}
    >
      <GridItem area="nav">
        <NavBar
          onSearch={(searchText) => {
            setHackerNewsQuery({
              ...hackerNewsQuery,
              maxStories: getMaxStories(searchText),
            });
          }}
        />
      </GridItem>
      <GridItem area="main">
        <Box paddingLeft={2}>
          <HackerNewsHeading newsQuery={hackerNewsQuery} />
        </Box>
        <HackerNewsGrid hackerNewsQuery={hackerNewsQuery}></HackerNewsGrid>
      </GridItem>
    </Grid>
  );
}

export default App;
