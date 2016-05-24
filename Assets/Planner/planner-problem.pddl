(define (problem rtg-prob1)
  (:domain rtg)
  	
  (:objects 
	person - person
	resource - resource
	building - building
	location1 - location
	location2 - location
	tool - tool
	)

  (:init 	
			(at person location1)
			(is-labourer person)
			(is-free location2)
	)
  
  (:goal 	
		(and
			(built-turfhut location2)
		)
	)
)