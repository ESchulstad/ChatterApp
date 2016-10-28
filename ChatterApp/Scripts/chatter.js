function follow(follower, author)
{
    if ($.inArray(author, follower.Following) == -1)
    {
        follower.Following.Add(author);
    }
}
