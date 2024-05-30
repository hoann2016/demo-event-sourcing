### Requirement
- Mongo db
- Kafka
- Sql Server(port: 1436)
### topic
socialMedia

### Command:
-Using mongo databse
- 7 command types:
  - NewPostCommand
  - EditMessageCommand
  - LikePostCommand
  - AddCommentCommand
  - EditCommentCommand
  - RemoveCommentCommand
  - DeletePostCommand
- Transfer to only one Aggregate is AggregatePost: AggregateRoot

Code flow:

**Step1 - config**: 
- In Program.cs the command register with Command Dispatcher.

**Step 2 - bulid command** 
- Request data send to controller -> we build the command then pass into the dispatcher like:  dispatcher.SendAsync(command)
- Inside SendAsynce, looking for in _handlers dictionary --> getting correct handler base the type of command (out Func<BaseCommand, Task> handler) == delegate so we can call the handler like : handler(command).
  
**Step 3 Processing command** 
- Base command we register in the command handler (step 1), nextstep happen in Command Handler.
  Inside method, based command provided, we build to Aggregate object

**What is  Aggregate:**
  - Id, Version, GetUncommitedChanges(), MarkChangesAsCommited(),ApplyChange,
  - RaiseEvent(BaseEvent), ReplayEvent(List of BaseEvent) --> call ApplyChange --> call Apply method by reflection technology.
  - The apply method with resonding param in PostAggregate will becalled.
  - next step is add this BaseEvent to the changes list.
  - Aggregate now include: _id, changes==GenUnCommitedChanges()
  - list changes= how many time theh RaiseEvent, or ReplayEvents call
 
**Step 4 aggregate root processing in Event sourcing method**
IEventSourcingHandler<PostAggregate> depend on IEventStore with: aggrateRootId, changes, Version from aggregate object, detail:
  - Check AggregateId is existed in write database or not.
  - Loop in the list of changes to create and event model then save event model to mongodb
  - Produce data to Kafka based configured topic

**Recap**
Transfer request data obj--> command obj-->
aggregate obj:
- Get PostAggregate
    - For Events:               
        - Call action -> Raising corresponding event. ex: aggregate.EditPost()
                   
--> Transfer PostAggreate to EventModel then Producer event to kafka + save to database.

**note**: if PostAggregate has more changes ==> producer many time.


### Request

Using a HostedService to consume data from kafka.
