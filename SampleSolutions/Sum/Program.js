const readline = require("node:readline");
const { stdin: input, stdout: output } = require("node:process");

const rl = readline.createInterface({ input, output });

const main = async () => {
  for await (const line of rl) {
    console.log(
      line
        .split(" ")
        .map((i) => +i)
        .reduce((a, b) => a + b, 0)
    );
    process.exit()
  }
};
main();
