import { Card, CardBody, Skeleton, SkeletonText } from "@chakra-ui/react";

const HackerNewsCardSkeleton = () => {
  return (
    <Card>
      <Skeleton height="200px"></Skeleton>
      <CardBody>
        <SkeletonText />
      </CardBody>
    </Card>
  );
};

export default HackerNewsCardSkeleton;
