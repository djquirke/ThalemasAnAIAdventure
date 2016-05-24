(define (domain rtg)
  (:requirements :strips :typing)
  
   (:types         
	person - object	
	location - object
	resource - object	
	building - object
	tool - object
	)
	
   (:predicates
		(at ?p - person ?l - location)
		(taught ?p - person)
		(attacking ?p - person)
		(got-money ?p - person)
		(is-free ?l - location)
		(person-created ?l - location)
		(persons-created ?l - location)
		
		;;person type
		(is-labourer ?p - person)
		(is-teacher ?p - person)
		(is-lumberjack ?p - person)
		(is-riffleman ?p - person)
		(is-miner ?p - person)
		(is-blacksmith ?p - person)
		(is-trader ?p - person)
		(is-carpenter ?p - person)
		(is-notriffleman ?p - person)
		
		;;resources
		(has-resource ?pl - location)
		(got-resource ?p - person)
		(is-forest ?r - resource)
		(is-stone ?r - resource)
		(is-wood ?r - resource)
		(is-iron ?r - resource)
		(is-timber ?r - resource)
		(is-ore ?r - resource)
		(is-coal ?r - resource)
		
		(got-stone ?p - person)
		(got-wood ?p - person)
		(got-iron ?p - person)
		(got-timber ?p - person)
		(got-ore ?p - person)
		(got-coal ?p - person)
		(got-mine ?p - person)
		
		;;at certain buildings
		(building-at ?b - building ?pl - location)
		(at-barracks ?p ?t - person ?pl - location)
		(at-turfhut ?p ?pp - person ?pl - location)
		(at-house ?p ?pp - person ?pl - location)
		(at-school ?p ?t - person ?pl - location)
		(at-mine ?p - person ?pl - location)
		(at-quarry ?p - person ?pl - location)
		
		;;building types
		(has-building ?l - location)
		(is-turfhut ?b - building)
		(is-house ?b - building)
		(is-school ?b - building)
		(is-mine ?b - building)
		(is-quarry ?b - building)
		(is-storage ?b - building)
		(is-market ?b - building)
		(is-sawmill ?b - building)
		(is-blacksmithB ?b - building)
		(is-smelter ?b - building)
		(is-barracks ?b - building)
		
		(built-turfhut ?l - location)
		(built-school ?l - location)
		(built-house ?l - location)
		(built-barracks ?l - location)
		(built-store ?l - location)
		(built-mine ?l - location)
		(built-quarry ?l - location)
		(built-sawmill ?l - location)
		(built-blacksmithB ?l - location)
		(built-smelter ?l - location)
		(built-market ?l - location)
		
		;;tool types
		(got-tool ?p - person)
		(is-axe ?t - tool)
		(is-cart ?t - tool)
		(is-riffle ?t - tool)
   )
   
   ;;ACTIONS
   (:action move
           :parameters (?p - person ?l-from ?l-to - location)
           :precondition (and (at ?p ?l-from))
           :effect (and (at ?p ?l-to) (not (at ?p ?l-from)))
	)
	
	(:action family
           :parameters (?p - person ?pp - person ?l - location ?b - building)
           :precondition (and (at ?p ?l) (at ?pp ?l) (is-turfhut ?b) (building-at ?b ?l))
           :effect (and (person-created ?l))
	)
	
	(:action family
           :parameters (?p - person ?pp - person ?l - location ?b - building )
           :precondition (and (at ?p ?l) (at ?pp ?l) (is-house ?b) (building-at ?b ?l))
           :effect (and (persons-created ?l))
	)
		
	(:action educate
           :parameters (?p - person ?t - person ?l - location ?b - building)
           :precondition (and (at ?p ?l) (at ?t ?l) (is-teacher ?t) (is-barracks ?b) (building-at ?b ?l))
           :effect (and (is-riffleman ?p))
	)
	
	(:action traintolumberjack
           :parameters (?p - person ?t - person ?l - location ?b - building)
           :precondition (and (at ?p ?l) (at ?t ?l) (is-teacher ?t) (is-school ?b) (building-at ?b ?l))
           :effect (and (is-lumberjack ?p))
	)
	
	(:action traintocarpenter
           :parameters (?p - person ?t - person ?l - location ?b - building)
           :precondition (and (at ?p ?l) (at ?t ?l) (is-teacher ?t) (is-school ?b) (building-at ?b ?l))
           :effect (and (is-carpenter ?p))
	)
	
	(:action traintolabourer
           :parameters (?p - person ?t - person ?l - location ?b - building)
           :precondition (and (at ?p ?l) (at ?t ?l) (is-teacher ?t) (is-school ?b) (building-at ?b ?l))
           :effect (and (is-labourer ?p))
	)
	
	(:action cuttree
           :parameters (?p - person ?l - location)
           :precondition (and (at ?p ?l) (has-resource ?l) (is-lumberjack ?p))
           :effect (and (got-timber ?p) (got-resource ?p) (not(has-resource ?l)))
	)
	
	(:action mine
           :parameters (?p - person ?b - building ?l - location)
           :precondition (and (at ?p ?l) (building-at ?b ?l) (is-mine ?b) (is-miner ?p))
           :effect (and (got-resource ?p) (not(has-resource ?l)))
	)
	
	(:action store
           :parameters (?p - person ?b - building ?l - location ?r - resource)
           :precondition (and (at ?p ?l) (building-at ?b ?l) (is-storage ?b) (got-resource ?p))
           :effect (and (got-money ?p) (has-resource ?l) (not(got-resource ?p)))
	)
		
		;;(got-ore ?p) (got-coal ?p)
	(:action smelt
           :parameters (?p - person ?b - building ?l - location)
           :precondition (and (at ?p ?l) (building-at ?b ?l) (is-smelter ?b))
           :effect (and (got-iron ?p) (got-resource ?p))
	)
	
	(:action quarry
           :parameters (?p - person ?b - building ?l - location)
           :precondition (and (at ?p ?l) (building-at ?b ?l) (is-quarry ?b) (is-labourer ?p))
           :effect (and (got-stone ?p) (got-resource ?p))
	)
	
	(:action sawwood
           :parameters (?p - person ?b - building ?l - location)
           :precondition (and (at ?p ?l) (building-at ?b ?l) (is-sawmill ?b) (is-lumberjack ?p) (got-timber ?p))
           :effect (and (got-wood ?p) (got-resource ?p))
	)	

	(:action maketool
           :parameters (?p - person ?b - building ?l - location)
           :precondition (and (at ?p ?l) (building-at ?b ?l) (is-blacksmithB ?b))
           :effect (and (got-tool ?p))
	)	
	
	(:action buy
           :parameters (?p - person ?b - building ?l - location)
           :precondition (and (at ?p ?l) (building-at ?b ?l) (is-market ?b) (got-resource ?p))
           :effect (and (got-resource ?p) (not(got-money ?p)) (not(has-resource ?l)))
	)
	
	(:action sell
           :parameters (?p - person ?b - building ?l - location ?r - resource)
           :precondition (and (at ?p ?l) (building-at ?b ?l) (is-market ?b) (got-resource ?p))
           :effect (and (got-money ?p) (has-resource ?l) (not(got-resource ?p)))
	)
	
	(:action combat
           :parameters (?p - person ?l - location)
           :precondition (and (at ?p ?l) (is-riffleman ?p))
           :effect (and (attacking ?p))
	)
	
	;;BUILDINGS
	(:action buildturfhut
           :parameters (?p - person ?l - location)
           :precondition (and (at ?p ?l) (is-labourer ?p))
           :effect (and (built-turfhut ?l))
	)
	
	(:action buildschool
           :parameters (?p - person ?pp - person ?l - location)
           :precondition (and (at ?p ?l) (at ?pp ?l) (got-stone ?p) (got-wood ?p) (got-iron ?p)
						(or(and(is-labourer ?p) (is-carpenter ?pp)) (and(is-carpenter ?p) (is-labourer ?p))))
           :effect (and (built-school ?l))
	)
	
	;;these are basically the same
	(:action buildhouse
           :parameters (?p - person ?pp - person ?l - location)
           :precondition (and (at ?p ?l) (at ?pp ?l) 
						(or(and(is-labourer ?p) (is-carpenter ?pp)) (and(is-carpenter ?p) (is-labourer ?p))) 
						(got-stone ?p) (got-wood ?p))
           :effect (and (built-house ?l))
	)
	(:action buildbarracks
           :parameters (?p - person ?pp - person ?l - location)
           :precondition (and (at ?p ?l) (at ?pp ?l) 
						(is-labourer ?p) (is-carpenter ?pp)
						(got-stone ?p) (got-wood ?p))
           :effect (and (built-barracks ?l))
	)	
	
	(:action buildstore
           :parameters (?p - person ?pp - person ?l - location)
           :precondition (and (at ?p ?l) (at ?pp ?l) 
						(or(and(is-labourer ?p) (is-carpenter ?pp)) (and(is-carpenter ?p) (is-labourer ?p))) 
						(got-stone ?p) (got-wood ?p))
           :effect (and (built-store ?l))
	)
	
	(:action buildmine
           :parameters (?p - person ?pp - person ?ppp - person ?l - location ?b - building)
           :precondition (and (at ?p ?l) (at ?pp ?l) (at ?ppp ?l) (building-at ?b ?l) 
						(is-labourer ?p) (is-carpenter ?pp) (is-blacksmith ?ppp) (is-blacksmithB ?b)
						(got-wood ?p) (got-iron ?p))
           :effect (and (built-mine ?l) (got-mine ?p))
	)
	
	(:action buildsmelter
           :parameters (?p - person ?l - location)
           :precondition (and (at ?p ?l) (is-labourer ?p) (got-stone ?p))
           :effect (and (built-smelter ?l))
	)
	
	(:action buildquarry
           :parameters (?p - person ?l - location)
           :precondition (and (at ?p ?l) (is-labourer ?p))
           :effect (and (built-quarry ?l))
	)
		
	;;these are basically the same
	(:action buildsawmill
           :parameters (?p - person ?l - location)
           :precondition (and (at ?p ?l) (is-labourer ?p) (got-iron ?p) (got-stone ?p) (got-timber ?p))
           :effect (and (built-sawmill ?l))
	)	
	(:action buildblacksmith
           :parameters (?p - person ?l - location)
           :precondition (and (at ?p ?l) (is-labourer ?p) (got-iron ?p) (got-stone ?p) (got-timber ?p))
           :effect (and (built-blacksmithB ?l))
	)
	
	(:action buildmarketstall
           :parameters (?p - person ?l - location)
           :precondition (and (at ?p ?l) (is-carpenter ?p) (got-wood ?p))
           :effect (and (built-market ?l))
	)
	
)
  