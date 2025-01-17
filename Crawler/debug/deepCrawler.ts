import fs from "fs";
import normalizeUrl from "normalize-url";
import { DeepCrawler } from "../src/crawler/DeepCrawler";

const fileName = `debug/spb.domain.ru_${new Date()
  .toISOString()
  .slice(0, 19)
  .replace(/[:T]/g, "-")}.txt`;

DeepCrawler.launch({
  links: ["http://demositerus.dev.qsupport.ru"],
  outgoingHosts: ["http://demositerus.dev.qsupport.ru"],
  concurrencyLevel: 2,
  blockedResources: ["stylesheet"],
  async handlePage(page) {
    fs.appendFileSync(
      fileName,
      `[${new Date()
        .toISOString()
        .slice(0, 19)
        .replace("T", " ")}] ${normalizeUrl(page.url())}\n`
    );
  }
});
