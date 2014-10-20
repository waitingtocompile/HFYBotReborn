.PHONY: clean All

All:
	@echo "----------Building project:[ HFYBotReborn - Debug ]----------"
	@$(MAKE) -f  "HFYBotReborn.mk"
clean:
	@echo "----------Cleaning project:[ HFYBotReborn - Debug ]----------"
	@$(MAKE) -f  "HFYBotReborn.mk" clean
